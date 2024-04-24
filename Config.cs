using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms; // Make sure your project supports this for Keys enum

namespace SpotiHotKey
{
    public static class ConfigManager
    {
        private static readonly string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpotiHotKey", "config.json");
        private static Config config;

        static ConfigManager()
        {
            LoadConfig();
        }

        private class Config
        {
            public List<List<Keys>> ShortcutKeys { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }

            public bool ShowNotifications { get; set; }
        }

        private static void LoadConfig()
        {
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                // Initialize with default values if config.json does not exist
                config = new Config
                {
                    ShortcutKeys = new List<List<Keys>>(),
                    ClientId = "",
                    ClientSecret = "",
                    ShowNotifications = true
                };
            }
        }

        public static List<List<Keys>> ShortcutKeys
        {
            get => config.ShortcutKeys;
            set
            {
                config.ShortcutKeys = value;
                SaveConfig();
            }
        }

        public static bool ShowNotifications
        {
            get => config.ShowNotifications;
            set
            {
                config.ShowNotifications = value;
                SaveConfig();
            }
        }

        public static string ClientId => config.ClientId;

        public static string ClientSecret => config.ClientSecret;

        private static void SaveConfig()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configFilePath, json);
        }
    }
}