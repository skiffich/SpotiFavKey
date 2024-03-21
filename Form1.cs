using System.Windows.Forms;

namespace SpotiHotKey
{
    public partial class SpotiFavKey : Form
    {
        private ShortcutController shortcutController = new ShortcutController();
        private SpotifyController spotifyController = new SpotifyController();

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

            spotifyController.Start();
            spotifyController.OnMessage += OnspotifyControllerMessage;

            notifyIcon.BalloonTipTitle = "Spotify Favorite Key";
            notifyIcon.BalloonTipText = "Spotify Favorite Key is running";
            notifyIcon.Text = "Spotify Favorite Key";

            showNotificationsCheck.Checked = ConfigManager.ShowNotifications;

            this.Hide();
        }

        public void OnShortcutSet(object source, OnShortcutSetArgs args)
        {
            string Text = "Shortcut " + (args.ShortcutIdx + 1).ToString() +" set to: " + args.Shortcut;
            switch (args.ShortcutIdx)
            {
                case 0:
                    lbl_hotkey_1.Text = "Shortcut set to: " + args.Shortcut;
                    break;
                case 1:
                    lbl_hotkey_2.Text = "Shortcut set to: " + args.Shortcut;
                    break;
                case 2:
                    lbl_hotkey_3.Text = "Shortcut set to: " + args.Shortcut;
                    break;
                case 3:
                    lbl_hotkey_4.Text = "Shortcut set to: " + args.Shortcut;
                    break;
            }

            Logger.LogToFile("Shortcut " + args.ShortcutIdx.ToString() + " set to: " + args.Shortcut);
        }

        private async void OnShortcutCall(object source, OnShortcutSetArgs args)
        {
            lbl_status.Text = "Shortcut " + (args.ShortcutIdx + 1).ToString() + " pressed!";
            Logger.LogToFile("Shortcut " + args.ShortcutIdx.ToString() + " pressed!");
            await spotifyController.SaveCurrentlyPlayingTrackToFavorites();
        }

        private void OnspotifyControllerMessage(object source, SpotifyControllerMessage args)
        {
            if (lbl_status.InvokeRequired)
            {
                lbl_status.Invoke(new MethodInvoker(delegate
                {
                    lbl_status.Text = args.Message;
                    Logger.LogToFile(args.Message);
                }));
            }
            else
            {
                lbl_status.Text = args.Message;
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

        private void SpotiFavKey_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            this.Hide();
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
                    notifyIcon.ShowBalloonTip(1000);
                }
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
                notifyIcon.BalloonTipText = "Spotify Favorite Key is still running";
                cb_saveList_1.Focus();
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
    }
}
