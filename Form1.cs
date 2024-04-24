using System.Windows.Forms;
using Microsoft.Win32;
using System;

namespace SpotiHotKey
{
    public partial class SpotiFavKey : Form
    {
        private ShortcutController shortcutController = new ShortcutController();
        private SpotifyController spotifyController = new SpotifyController();

        private const string appName = "SpotiFavKey";

        public SpotiFavKey()
        {
            InitializeComponent();

            for (int i = 0; i < 4; i++)
            {
                var shortcutKeys = shortcutController.GetShortcut(i);
                if (shortcutKeys.Length > 0)
                {
                    string Text = "Shortcut " + (i + 1).ToString() + " set to: " + shortcutKeys;
                    switch (i)
                    {
                        case 0:
                            lbl_hotkey_1.Text = Text;
                            break;
                        case 1:
                            lbl_hotkey_2.Text = Text;
                            break;
                        case 2:
                            lbl_hotkey_3.Text = Text;
                            break;
                        case 3:
                            lbl_hotkey_4.Text = Text;
                            break;
                    }
                    Logger.LogToFile("Shortcut " + i.ToString() + " set to: " + shortcutKeys);
                }
            }

            shortcutController.OnShortcutSetEvent += OnShortcutSet;
            shortcutController.OnShortcutCallEvent += OnShortcutCall;
            shortcutController.OnShortcutMessage += OnMessage;

            spotifyController.Start();
            spotifyController.OnMessage += OnMessage;
            spotifyController.OnAuthSuccess += SpotiFavKey_Load;
            spotifyController.OnNotification += OnSpotifyNotification;

            notifyIcon.BalloonTipTitle = "Spotify Favorite Key";
            notifyIcon.BalloonTipText = "Spotify Favorite Key is running";
            notifyIcon.Text = "Spotify Favorite Key";

            showNotificationsCheck.Checked = ConfigManager.ShowNotifications;

            RunOnStartupCheckBox.Checked = IsAutoStartEnabled();

            this.Hide();
        }

        public void OnShortcutSet(object source, OnShortcutSetArgs args)
        {
            string Text = "Shortcut " + (args.ShortcutIdx + 1).ToString() + " set to: " + args.Shortcut;
            if (args.Shortcut == "No hotkey Set")
            {
                Text = args.Shortcut;
            }
            switch (args.ShortcutIdx)
            {
                case 0:
                    lbl_hotkey_1.Text = Text;
                    break;
                case 1:
                    lbl_hotkey_2.Text = Text;
                    break;
                case 2:
                    lbl_hotkey_3.Text = Text;
                    break;
                case 3:
                    lbl_hotkey_4.Text = Text;
                    break;
            }

            Logger.LogToFile("Shortcut " + args.ShortcutIdx.ToString() + " set to: " + args.Shortcut);
        }

        private async void OnShortcutCall(object source, OnShortcutSetArgs args)
        {
            //lbl_status.Text = "Shortcut " + (args.ShortcutIdx + 1).ToString() + " pressed!";
            Logger.LogToFile("Shortcut " + args.ShortcutIdx.ToString() + " pressed!");
            var playingTrack = await spotifyController.GetCurrentlyPlayingTrackId();
            string checkInListId = "";
            string checkInListName = "";
            ComboBox selectedComboBox = null;

            switch (args.ShortcutIdx)
            {
                case 0:
                    selectedComboBox = cb_saveList_1;
                    break;
                case 1:
                    selectedComboBox = cb_saveList_2;
                    break;
                case 2:
                    selectedComboBox = cb_saveList_3;
                    break;
                case 3:
                    selectedComboBox = cb_saveList_4;
                    break;
            }
            if (selectedComboBox != null)
            {
                var selectedItem = selectedComboBox.SelectedItem as dynamic;
                checkInListId = selectedItem?.playlistId;
                checkInListName = selectedItem?.playlistName;
            }

            if (checkInListId.Length > 0)
            {
                if (checkInListId == "Favorites")
                {
                    if (await spotifyController.IsTrackInFavoritesAsync(playingTrack.trackId))
                    {
                        await spotifyController.RemoveTrackFromFavoritesAsync(playingTrack.trackId, playingTrack.trackName);
                    }
                    else
                    {
                        await spotifyController.SaveCurrentlyPlayingTrackToFavorites();
                    }
                }
                else
                {
                    if (await spotifyController.IsTrackInPlaylistAsync(checkInListId, playingTrack.trackId))
                    {
                        await spotifyController.RemoveTrackFromPlaylistAsync(checkInListId, checkInListName, playingTrack.trackId, playingTrack.trackName);
                    }
                    else
                    {
                        await spotifyController.AddTrackToPlaylistAsync(checkInListId, checkInListName, playingTrack.trackId, playingTrack.trackName);
                    }
                }
            }
        }

        private void OnMessage(object source, EventArgs args)
        {
            String Text = "";
            if (source == spotifyController)
            {
                var spMessage = args as SpotifyControllerMessage;
                Text = spMessage.Message;
            }
            else if (source == shortcutController)
            {
                var spMessage = args as OnShortcutSetArgs;
                Text = spMessage.Shortcut;
            }
            if (lbl_status.InvokeRequired)
            {
                lbl_status.Invoke(new MethodInvoker(delegate
                {
                    lbl_status.Text = Text;
                    Logger.LogToFile(Text);
                }));
            }
            else
            {
                lbl_status.Text = Text;
            }
        }

        private void OnSpotifyNotification(object source, SpotifyControllerMessage args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => OnSpotifyNotification(source, args)));
            }
            else
            {
                if (ConfigManager.ShowNotifications)
                {
                    notifyIcon.BalloonTipText = args.Message;
                    notifyIcon.ShowBalloonTip(1000);
                }
            }
        }

        private void btn_set_hotkey_Click(object sender, EventArgs e)
        {
            if (sender == btn_set_hotkey_1)
            {
                lbl_hotkey_1.Text = "Enter Shortcut 1";
                Logger.LogToFile("Enter Shortcut 0");
                shortcutController.SetShortcut(0);
            }
            else if (sender == btn_set_hotkey_2)
            {
                lbl_hotkey_2.Text = "Enter Shortcut 2";
                Logger.LogToFile("Enter Shortcut 1");
                shortcutController.SetShortcut(1);
            }
            else if (sender == btn_set_hotkey_3)
            {
                lbl_hotkey_3.Text = "Enter Shortcut 3";
                Logger.LogToFile("Enter Shortcut 2");
                shortcutController.SetShortcut(2);
            }
            else if (sender == btn_set_hotkey_4)
            {
                lbl_hotkey_4.Text = "Enter Shortcut";
                Logger.LogToFile("Enter Shortcut 3");
                shortcutController.SetShortcut(3);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        private async void SpotiFavKey_Load(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(SpotiFavKey_Load), new object[] { sender, e });
            }
            else
            {
                List<(string playlistName, string playlistId)> playlists = await spotifyController.GetUserPlaylistsAsync();
                var playlistItems1 = playlists.Select(p => new { p.playlistName, p.playlistId }).ToList();
                var playlistItems2 = playlists.Select(p => new { p.playlistName, p.playlistId }).ToList();
                var playlistItems3 = playlists.Select(p => new { p.playlistName, p.playlistId }).ToList();
                var playlistItems4 = playlists.Select(p => new { p.playlistName, p.playlistId }).ToList();
                if (playlists.Count > 0)
                {
                    int choosenItem = 0;
                    cb_saveList_2.DataSource = playlistItems2;
                    cb_saveList_2.DisplayMember = "playlistName";
                    cb_saveList_2.ValueMember = "playlistId";
                    cb_saveList_2.SelectedItem = playlistItems2[choosenItem];
                    if (choosenItem + 1 < playlistItems2.Count) { choosenItem++; }

                    cb_saveList_3.DataSource = playlistItems3;
                    cb_saveList_3.DisplayMember = "playlistName";
                    cb_saveList_3.ValueMember = "playlistId";
                    cb_saveList_3.SelectedItem = playlistItems3[choosenItem];
                    if (choosenItem + 1 < playlistItems3.Count) { choosenItem++; }

                    cb_saveList_4.DataSource = playlistItems4;
                    cb_saveList_4.DisplayMember = "playlistName";
                    cb_saveList_4.ValueMember = "playlistId";
                    cb_saveList_4.SelectedItem = playlistItems4[choosenItem];
                }

                playlistItems1.Add(new { playlistName = "Favorites", playlistId = "Favorites" });
                cb_saveList_1.DataSource = playlistItems1;
                cb_saveList_1.DisplayMember = "playlistName";
                cb_saveList_1.ValueMember = "playlistId";
                cb_saveList_1.SelectedItem = playlistItems1.Last();
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void SpotiFavKey_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
                if (ConfigManager.ShowNotifications)
                {
                    notifyIcon.BalloonTipText = "Spotify Favorite Key is running";
                    notifyIcon.ShowBalloonTip(1000);
                }
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
                cb_saveList_2.Focus();
            }
        }

        private void SpotiFavKey_Shown(object sender, EventArgs e)
        {
            //to minimize window
            this.WindowState = FormWindowState.Minimized;

            //to hide from taskbar
            this.Hide();
        }

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Show();
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
            }
        }

        private void showNotificationsCheck_CheckStateChanged(object sender, EventArgs e)
        {
            ConfigManager.ShowNotifications = showNotificationsCheck.Checked;
        }

        private void RunOnStartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RunOnStartupCheckBox.Checked)
                EnableAutoStart();
            else
                DisableAutoStart();
        }

        private bool IsAutoStartEnabled()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                Logger.LogToFile("Checking if autorun is enabled");
                return key.GetValue(appName) != null;
            }
            Logger.LogToFile("Failed OpenSubKey while IsAutoStartEnabled");
        }

        private void EnableAutoStart()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                Logger.LogToFile("Enabling autorun");
                key.SetValue(appName, Application.ExecutablePath);
                return;
            }
            Logger.LogToFile("Failed OpenSubKey while EnableAutoStart");
        }

        private void DisableAutoStart()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                Logger.LogToFile("Disabling autorun");
                key.DeleteValue(appName, false);
                return;
            }
            Logger.LogToFile("Failed OpenSubKey while DisableAutoStart");
        }
    }
}
