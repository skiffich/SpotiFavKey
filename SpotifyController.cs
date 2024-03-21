using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace SpotiHotKey
{
    public class TokenInfo
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsExpired()
        {
            return (DateTime.UtcNow - CreatedAt).TotalSeconds > ExpiresIn;
        }
    }

    public class SpotifyControllerMessage : EventArgs
    {
        public string Message { get; private set; }
        public bool Result { get; private set; }

        public SpotifyControllerMessage(string message = "", bool resul = true)
        {
            Message = message;
            Result = resul;
        }
    }

    public class SpotifyController
    {
        private const string TokenInfoFile = "token_info.json";
        private readonly string clientId = "1747b3ed9d0740c9a6ff29f1ca6997b8";
        private readonly string clientSecret = "c5ca7a8a2751480a86d63737f28e4c99";
        private readonly string redirectUri = "http://localhost:5000/callback";
        private readonly HttpListener httpListener = new HttpListener();
        private string accessToken;

        public delegate void OnSpotifyControllerMessageHandler(object source, SpotifyControllerMessage args);
        public event OnSpotifyControllerMessageHandler OnMessage;
        public event OnSpotifyControllerMessageHandler OnAuthSuccess;
        public event OnSpotifyControllerMessageHandler OnNotification;

        public SpotifyController()
        {
            if (!(string.IsNullOrEmpty(ConfigManager.ClientId) || string.IsNullOrEmpty(ConfigManager.ClientSecret)))
            {
                clientId = ConfigManager.ClientId;
                clientSecret = ConfigManager.ClientSecret;
            }
        }

        public void Start()
        {
            var tokenInfo = LoadTokenInfo();
            if (tokenInfo != null)
            {
                RefreshTokenIfExpired(tokenInfo);
                accessToken = tokenInfo.AccessToken;
            }
            else
            {
                httpListener.Prefixes.Add("http://localhost:5000/");
                httpListener.Start();
                Task.Run(() => Authenticate());
            }
        }

        private void SaveTokenInfo(TokenInfo tokenInfo)
        {
            string json = JsonConvert.SerializeObject(tokenInfo);
            File.WriteAllText(TokenInfoFile, json);
        }

        private TokenInfo LoadTokenInfo()
        {
            if (!File.Exists(TokenInfoFile))
            {
                return null;
            }

            string json = File.ReadAllText(TokenInfoFile);
            return JsonConvert.DeserializeObject<TokenInfo>(json);
        }

        private async Task RefreshTokenIfExpired(TokenInfo tokenInfo)
        {
            if (tokenInfo == null || !tokenInfo.IsExpired())
            {
                return;
            }

            // Use WebClient or HttpClient to refresh the token
            // Update the accessToken and save the new token info
            using var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            var response = client.UploadString("https://accounts.spotify.com/api/token", "POST", $"grant_type=refresh_token&refresh_token={tokenInfo.RefreshToken}");
            var tokenResponse = JsonConvert.DeserializeObject<TokenInfo>(response);
            accessToken = tokenResponse.AccessToken;
            SaveTokenInfo(tokenResponse); // Save the new token info
            OnMessage(this, new SpotifyControllerMessage("Token refreshed"));
        }

        private void Authenticate()
        {
            // Generate the Spotify authentication URL
            var queryParams = new System.Collections.Generic.Dictionary<string, string>
        {
            {"response_type", "code"},
            {"client_id", clientId},
            {"scope", "user-read-currently-playing user-library-read user-library-modify playlist-read-private playlist-modify-public playlist-modify-private" },
            {"redirect_uri", redirectUri}
        };
            var authUrl = "https://accounts.spotify.com/authorize?" + BuildQueryString(queryParams);

            // Open the user's browser to request authorization
            Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });

            // Start listening for the redirect callback
            ListenForAuthResponse();
        }

        private async void ListenForAuthResponse()
        {
            try
            {
                while (httpListener.IsListening)
                {
                    var context = await httpListener.GetContextAsync();
                    if (context.Request.Url.AbsolutePath == "/callback")
                    {
                        var code = context.Request.QueryString["code"];
                        await ExchangeCodeForToken(code);

                        string responseString = "<html><h2>Authorization Successful!</h2><p>You can now close this window.</p></html>";
                        OnMessage(this, new SpotifyControllerMessage("Authorization Successful"));
                        OnAuthSuccess(this, new SpotifyControllerMessage());
                        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        context.Response.ContentLength64 = buffer.Length;
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Close();
                        httpListener.Stop();
                    }
                }
            }
            catch
            {
                Logger.LogToFile("ListenForAuthResponse exception");
            }
        }

        private async Task ExchangeCodeForToken(string code)
        {
            var tokenUrl = "https://accounts.spotify.com/api/token";
            using var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            var response = client.UploadString(tokenUrl, "POST", $"grant_type=authorization_code&code={code}&redirect_uri={redirectUri}");
            var tokenResponse = JsonConvert.DeserializeObject<dynamic>(response);
            accessToken = tokenResponse.access_token;
            var tokenInfo = new TokenInfo
            {
                AccessToken = tokenResponse.access_token,
                TokenType = tokenResponse.token_type,
                RefreshToken = tokenResponse.refresh_token,
                ExpiresIn = tokenResponse.expires_in
            };
            SaveTokenInfo(tokenInfo);
        }

        private string BuildQueryString(System.Collections.Generic.Dictionary<string, string> queryParams)
        {
            var array = new System.Collections.Generic.List<string>();
            foreach (var key in queryParams.Keys)
            {
                array.Add($"{System.Web.HttpUtility.UrlEncode(key)}={System.Web.HttpUtility.UrlEncode(queryParams[key])}");
            }
            return string.Join("&", array);
        }

        public async Task<(string trackName, string artist, string trackId)> GetCurrentlyPlayingTrackId()
        {
            string trackName = "";
            string artist = "";
            string trackId = "";
            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                var result = await client.DownloadStringTaskAsync("https://api.spotify.com/v1/me/player/currently-playing");
                dynamic currentlyPlaying = JsonConvert.DeserializeObject<dynamic>(result);
                if (currentlyPlaying != null)
                {
                    trackName = currentlyPlaying.item.name;
                    artist = currentlyPlaying.item.artists[0].name;
                    trackId = currentlyPlaying.item.id;
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"GetCurrentlyPlayingTrackId exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage($"Get Currently Playing Track failed"));
            }
            return (trackName, artist, trackId);
        }

        public async Task<List<(string playlistName, string playlistId)>> GetUserPlaylistsAsync()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                Logger.LogToFile("Access Token is not available. Authenticate first.");
                return new List<(string, string)>();
            }

            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                var result = await client.DownloadStringTaskAsync("https://api.spotify.com/v1/me/playlists");
                dynamic playlists = JsonConvert.DeserializeObject<dynamic>(result);

                List<(string, string)> resultList = new List<(string, string)>(); ;

                if (playlists != null && playlists.items != null)
                {
                    foreach (var playlist in playlists.items)
                    {
                        string playlistName = playlist.name;
                        string playlistId = playlist.id;
                        resultList.Add((playlistName, playlistId));
                        // Here, you can collect or print out the playlist information as needed
                        //OnMessage(this, new SpotifyControllerMessage($"Playlist: {playlistName} (ID: {playlistId})"));
                    }

                    return resultList;
                }
                else
                {
                    Logger.LogToFile("No playlists found or failed to retrieve playlists.");
                    OnMessage(this, new SpotifyControllerMessage("No playlists found or failed to retrieve playlists."));
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"GetUserPlaylistsAsync exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage("Failed to retrieve playlists.", false));
            }
            return new List<(string, string)>();
        }

        public async Task<bool> IsTrackInPlaylistAsync(string playlistId, string trackId)
        {
            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                var result = await client.DownloadStringTaskAsync($"https://api.spotify.com/v1/playlists/{playlistId}/tracks");
                dynamic playlistTracks = JsonConvert.DeserializeObject<dynamic>(result);

                foreach (var item in playlistTracks.items)
                {
                    if (item.track.id == trackId)
                    {
                        return true; // Track found in playlist
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"IsTrackInPlaylistAsync exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage($"IsTrackInPlaylistAsync failed"));
            }

            return false; // Track not found in playlist or an error occurred
        }

        public async Task AddTrackToPlaylistAsync(string playlistId, string playlistName, string trackId, string trackName)
        {
            try
            {
                string trackUri = $"spotify:track:{trackId}";
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string payload = JsonConvert.SerializeObject(new { uris = new[] { trackUri } });
                await client.UploadStringTaskAsync($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", "POST", payload);

                Logger.LogToFile($"AddTrackToPlaylistAsync : Track {trackName} added to playlist {playlistName} successfully.");
                OnMessage(this, new SpotifyControllerMessage($"Track {trackName} added to playlist {playlistName} successfully."));
                OnNotification(this, new SpotifyControllerMessage($"Track {trackName} added to playlist {playlistName}"));
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"AddTrackToPlaylistAsync exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage($"Track add failed"));
            }
        }

        public async Task RemoveTrackFromPlaylistAsync(string playlistId, string playlistName, string trackId, string trackName)
        {
            try
            {
                string trackUri = $"spotify:track:{trackId}";
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string payload = JsonConvert.SerializeObject(new { tracks = new[] { new { uri = trackUri } } });
                await client.UploadStringTaskAsync($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", "DELETE", payload);

                // Optionally, notify via OnMessage
                OnMessage(this, new SpotifyControllerMessage($"Track {trackName} removed from playlist {playlistName} successfully."));
                OnNotification(this, new SpotifyControllerMessage($"Track {trackName} removed from playlist {playlistName}"));
                Logger.LogToFile($"RemoveTrackFromPlaylistAsync : Track { trackName} removed from playlist { playlistName} successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"RemoveTrackFromPlaylistAsync exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage($"Track remove failed"));
            }
        }

        public async Task SaveCurrentlyPlayingTrackToFavorites()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                OnMessage(this, new SpotifyControllerMessage("Access Token is not available. Authenticate first."));
                return;
                //throw new InvalidOperationException("Access Token is not available. Authenticate first.");
            }

            // Get the ID of the currently playing track
            var (trackName, artist, currentlyPlayingTrackId) = await GetCurrentlyPlayingTrackId();
            if (string.IsNullOrEmpty(currentlyPlayingTrackId))
            {
                OnMessage(this, new SpotifyControllerMessage("No track is currently playing or failed to retrieve track ID."));
                return;
                //throw new InvalidOperationException("No track is currently playing or failed to retrieve track ID.");
            }

            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                var response = await client.UploadStringTaskAsync($"https://api.spotify.com/v1/me/tracks?ids={currentlyPlayingTrackId}", "PUT", "");
                OnMessage(this, new SpotifyControllerMessage($"'{trackName}' by '{artist}' has been added to your favorites."));
                OnNotification(this, new SpotifyControllerMessage($"'{trackName}' by '{artist}' has been added to your favorites."));
                // If the response is successful, the track has been added to the favorites
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"SaveCurrentlyPlayingTrackToFavorites exception: {ex.Message}");
            }
        }
        
        public async Task RemoveTrackFromFavoritesAsync(string trackId, string trackName)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                OnMessage(this, new SpotifyControllerMessage("Access Token is not available. Authenticate first.", false));
                return;
            }

            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                await client.UploadStringTaskAsync($"https://api.spotify.com/v1/me/tracks?ids={trackId}", "DELETE", string.Empty);

                OnMessage(this, new SpotifyControllerMessage($"Track {trackName} has been removed from your favorites."));
                OnNotification(this, new SpotifyControllerMessage($"Track {trackName} has been removed from your favorites."));
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"RemoveTrackFromFavoritesAsync exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage("Failed to remove track from favorites.", false));
            }
        }

        public async Task<bool> IsTrackInFavoritesAsync(string trackId)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                OnMessage(this, new SpotifyControllerMessage("Access Token is not available. Authenticate first.", false));
                return false;
            }

            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                var result = await client.DownloadStringTaskAsync($"https://api.spotify.com/v1/me/tracks/contains?ids={trackId}");
                bool[] trackIsInFavorites = JsonConvert.DeserializeObject<bool[]>(result);

                if (trackIsInFavorites.Length > 0)
                {
                    return trackIsInFavorites[0];
                }
            }
            catch (Exception ex)
            {
                Logger.LogToFile($"IsTrackInFavoritesAsync exception: {ex.Message}");
                OnMessage(this, new SpotifyControllerMessage($"Failed to check if track {trackId} is in favorites.", false));
            }

            return false; // Якщо виникла помилка або трек не знайдено серед улюблених, повертаємо false
        }

        ~SpotifyController() 
        {
            httpListener.Stop();
        }
    }
}
