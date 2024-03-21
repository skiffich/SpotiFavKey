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

            var shortcutKeys = shortcutController.GetShortcut();
            if (shortcutKeys.Length > 0)
            {
                lbl_hotkey.Text = "Shortcut set to: " + shortcutKeys;
                Logger.LogToFile("Shortcut set to: " + shortcutKeys);
            }

            shortcutController.OnShortcutSetEvent += OnShortcutSet;
            shortcutController.OnShortcutCallEvent += OnShortcutCall;

            spotifyController.Start();
            spotifyController.OnMessage += OnspotifyControllerMessage;

            notifyIcon.BalloonTipTitle = "Spotify Favorite Key";
            notifyIcon.BalloonTipText = "Spotify Favorite Key is running";
            notifyIcon.Text = "Spotify Favorite Key";

            this.Hide();
        }

        public void OnShortcutSet(object source, OnShortcutSetArgs args)
        {
            lbl_hotkey.Text = "Shortcut set to: " + args.Shortcut;
            Logger.LogToFile("Shortcut set to: " + args.Shortcut);
        }

        private async void OnShortcutCall(object source, OnShortcutSetArgs args)
        {
            lbl_status.Text = "Shortcut pressed!";
            Logger.LogToFile("Shortcut pressed!");
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
            lbl_hotkey.Text = "Enter Shortcut";
            Logger.LogToFile("Enter Shortcut");
            shortcutController.SetShortcut();
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
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
                notifyIcon.BalloonTipText = "Spotify Favorite Key is still running";
            }
        }

        private void SpotiFavKey_Shown(object sender, EventArgs e)
        {
            //to minimize window
            this.WindowState = FormWindowState.Minimized;

            //to hide from taskbar
            this.Hide();
        }
    }
}
