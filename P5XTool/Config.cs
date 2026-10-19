using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace P5XTool
{
    public class Config
    {
        public class User : ICloneable
        {
            public string? AutoPath { get; set; } = string.Empty;
            public bool AutoLoad { get; set; } = false;
            public User Clone()
            {
                return (User)MemberwiseClone();
            }
            object ICloneable.Clone()
            {
                return Clone();
            }
        }
        public List<string> MRU { get; set; } = [];
        public User UserConfig { get; set; } = new();

        private static Config? _instance;
        public static Config Instance => _instance ??= LoadConfig();
        private static readonly string ConfigPath = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory)!, "config.json");
        private static Config LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                try
                {
                    var deSer = JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigPath));
                    if (deSer != null) return deSer;
                }
                catch { }
            }
            return new();
        }

        public static void Invalidate()
        {
            _instance = null;
        }
        public static void SaveConfig()
        {
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(Instance));
        }
        public static List<string> GetRecents()
        {
            return Instance.MRU;
        }
        public static void AddRecentPath(string path)
        {
            var recentFiles = Instance.MRU;
            var existIdx = recentFiles.IndexOf(path);
            if (existIdx == 0) return;
            if (existIdx != -1) recentFiles.RemoveAt(existIdx);
            recentFiles.Insert(0, path);
            if (recentFiles.Count > 5)
            {
                recentFiles.RemoveAt(recentFiles.Count - 1);
            }
            SaveConfig();
        }
        public static string? GetAutoLoadPath()
        {
            var conf = Instance.UserConfig;
            if (conf.AutoLoad && !string.IsNullOrEmpty(conf.AutoPath))
            {
                return conf.AutoPath;
            }
            return null;
        }
        public static User GetUserConfig() {
            return Instance.UserConfig;
        }
        public static void SetUserConfig(User user)
        {
            Instance.UserConfig = user;
            SaveConfig();
        }
    }
}
