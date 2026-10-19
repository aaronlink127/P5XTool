using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public static class I18NAssetMap
    {
        public static bool Enabled = true;
        public static string? CurrentLocale { get; private set; }
        public static string? MainLanguage;
        private static Dictionary<string, string>? assetMap = null;
        private static Dictionary<string, string> i18nBaseMap = [];
        private static Dictionary<string, Dictionary<string, string>> localeMaps = [];
        //private static List<string> langList = [];
        public static IEnumerable<string> LangList => localeMaps.Keys;
        public static void Init()
        {
            Enabled = true;
            LoadAssetMap();
            LoadSpriteTextureMap();
            SetCurrentLocale(MainLanguage);
        }
        public static void Reset()
        {
            Enabled = false;
            CurrentLocale = null;
            assetMap = null;
            i18nBaseMap.Clear();
            localeMaps.Clear();
            MainLanguage = null;
        }
        public static void LoadAssetMap()
        {
            var assetMapXml = AssetManager.LoadBytes("I18NAssetMap.xml");
            if (assetMapXml == null) return;
            var xmlDoc = new System.Xml.XmlDocument();
            using MemoryStream ms = new(assetMapXml);
            xmlDoc.Load(ms);
            var root = xmlDoc.DocumentElement;
            if (root == null) return;
            var map = new Dictionary<string, string>();
            MainLanguage = root.SelectSingleNode("MainLanguage")?.InnerText;
            if (string.IsNullOrEmpty(MainLanguage))
            {
                MainLanguage = null;
                return;
            }
            var multiLangNodes = root.SelectNodes("MultiLanguages/string");
            localeMaps[MainLanguage] = [];
            if (multiLangNodes != null)
            {
                foreach (System.Xml.XmlNode langNode in multiLangNodes)
                {
                    localeMaps[langNode.InnerText] = [];
                }
            }
            var assetNodes = root.SelectNodes("Assets/AssetInfo");
            if (assetNodes != null)
            {
                foreach (System.Xml.XmlNode assetNode in assetNodes)
                {
                    var assetName = assetNode.Attributes?["AssetName"]?.Value;
                    //var assetType = assetNode.Attributes?["AssetType"]?.Value;
                    if (assetName != null)
                    {
                        if (assetName.Contains("_I18N"))
                        {
                            var key = assetName.Replace("_I18N", "");
                            i18nBaseMap[key] = assetName;
                        }
                        else
                        {
                            foreach (var lang in LangList)
                            {
                                var langStr = $"_{lang}";
                                if (assetName.Contains(langStr))
                                {
                                    var key = assetName.Replace(langStr, "");
                                    localeMaps[lang][key] = assetName;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void LoadSpriteTextureMap()
        {
            var packedSpritesAndTextures = AssetManager.LoadText("PackedSpritesAndTextures.xml");
            if (packedSpritesAndTextures == null) return;
            var lines = packedSpritesAndTextures.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var fileName in lines)
            {
                if (fileName.EndsWith("-i18n.png"))
                {
                    var key = fileName.Substring(0, fileName.Length - "-i18n.png".Length) + ".png";
                    i18nBaseMap[key] = fileName;
                }
                else
                {
                    foreach (var lang in LangList)
                    {
                        var suffix = $"-i18n_{lang}.png";
                        if (fileName.EndsWith(suffix))
                        {
                            var noSuffix = fileName.Substring(0, fileName.Length - suffix.Length);
                            var key = noSuffix + ".png";
                            var key2 = noSuffix + "-i18n.png";
                            localeMaps[lang][key] = fileName;
                            localeMaps[lang][key2] = fileName;
                            break;
                        }
                    }
                }
            }
        }
        public static void SetCurrentLocale(string? locale)
        {
            CurrentLocale = locale;
            if (locale == null)
                assetMap = null;
            else
                assetMap = localeMaps.GetValueOrDefault(locale);
        }
        public static string GetAssetName(string key)
        {
            if (!Enabled) return key;
            if (assetMap != null && assetMap.TryGetValue(key, out var name))
            {
                return name;
            }
            if (i18nBaseMap.TryGetValue(key, out var baseName))
            {
                return baseName;
            }
            return key;
        }
        public static List<string> GetAllAssetNames(string key)
        {
            List<string> names = [key];
            if (i18nBaseMap.TryGetValue(key, out var baseName))
            {
                names.Add(baseName);
            }
            foreach (var kv in localeMaps)
            {
                var lang = kv.Key;
                var localeMap = kv.Value;
                if (localeMap.TryGetValue(key, out var localeName))
                {
                    names.Add(localeName);
                }
            }
            return names;
        }
    }
}
