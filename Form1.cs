namespace SpotiHotKey
{
    public partial class Form1 : Form
    {
        private ShortcutController shortcutController = new ShortcutController();
        private SpotifyController spotifyController = new SpotifyController();

        public Form1()
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
    }
}
