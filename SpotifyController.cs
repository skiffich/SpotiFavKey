using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;

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
            {"scope", "user-read-currently-playing user-library-modify" },
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

        private async Task<string> GetCurrentlyPlayingTrack()
        {
            try
            {

                if (string.IsNullOrEmpty(accessToken))
                {
                    OnMessage(this, new SpotifyControllerMessage("Access Token is not available. Authenticate first."));
                    return null;
                    // throw new InvalidOperationException("Access Token is not available. Authenticate first.");
                }

                using var client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                var result = await client.DownloadStringTaskAsync("https://api.spotify.com/v1/me/player/currently-playing");
                dynamic currentlyPlaying = JsonConvert.DeserializeObject<dynamic>(result);
                return currentlyPlaying.item.name;
            }
            catch
            {
                Logger.LogToFile("GetCurrentlyPlayingTrack exception");
            }
            return "";
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
                // If the response is successful, the track has been added to the favorites
            }
            catch
            {
                Logger.LogToFile("SaveCurrentlyPlayingTrackToFavorites exception");
            }
        }

        private async Task<(string trackName, string artist, string trackId)> GetCurrentlyPlayingTrackId()
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
            catch
            {
                Logger.LogToFile("GetCurrentlyPlayingTrackId exception");
            }
            return (trackName, artist, trackId);
        }

        ~SpotifyController() 
        {
            httpListener.Stop();
        }
    }
}
