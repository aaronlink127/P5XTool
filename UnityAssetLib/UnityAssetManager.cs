using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{

    public class UnityAssetManager
    {
        private static readonly Dictionary<string, SerializedFile> loadedFiles = [];
        private static readonly Dictionary<string, byte[]> loadedBins = [];

        public static int LoadedFileCount => loadedFiles.Count;
        public static int LoadedBinCount => loadedBins.Count;

        public static SerializedFile? GetLoadedFile(string path)
        {
            if (loadedFiles.TryGetValue(path, out var sf))
            {
                return sf;
            }
            return null;
        }

        public static void RegisterFile(string path, SerializedFile file)
        {
            loadedFiles[path] = file;
            file.path = path;
        }

        public static AssetBundle LoadAssetBundle(byte[] data)
        {
            var aB = new AssetBundle();
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            var b = new BundleFile(ms);
            bool foundAssetBundle = false;
            foreach (var file in b.files)
            {
                var cabName = Path.GetFileNameWithoutExtension(file.path);
                var fullPath = "archive:/" + cabName + "/" + file.path;
                if (!aB.cabNames.Contains(cabName))
                {
                    aB.cabNames.Add(cabName);
                }
                if (file.path.EndsWith(".resS") || file.path.EndsWith(".resource"))
                {
                    loadedBins[fullPath] = file.data;
                }
                else
                {
                    SerializedFile sf = new(file.data);
                    RegisterFile(fullPath, sf);
                    if (!foundAssetBundle)
                    {
                        var abData = sf.GetObjectDict(1);
                        if (abData == null || abData["__type"] as string != "AssetBundle")
                        {
                            abData = sf.GetObjectDict(2);
                        }
                        aB.LoadDataFrom(abData!, sf);
                        foundAssetBundle = true;
                    }
                }
            }
            return aB;
        }

        public static ReadOnlySpan<byte> LoadStreamedData(StreamingInfo info)
        {
            if (loadedBins.TryGetValue(info.path, out var data))
            {
                return data.AsSpan((int)info.offset, (int)info.size);
            }
            throw new FileNotFoundException(info.path);
        }

        public static void UnloadArchive(string cabName)
        {
            var prefix = "archive:/" + cabName + "/";
            foreach (var key in loadedFiles.Keys.Where(k => k.StartsWith(prefix)).ToList())
            {
                loadedFiles.Remove(key);
            }
            foreach (var key in loadedBins.Keys.Where(k => k.StartsWith(prefix)).ToList())
            {
                loadedBins.Remove(key);
            }
        }

        public static List<SerializedFile> GetFilesInBundle(AssetBundle bundle)
        {
            var result = new List<SerializedFile>();
            foreach (var cabName in bundle.cabNames)
            {
                var prefix = "archive:/" + cabName + "/";
                foreach (var kv in loadedFiles)
                {
                    if (kv.Key.StartsWith(prefix))
                    {
                        result.Add(kv.Value);
                    }
                }
            }
            return result;
        }

        public static void Init(string gamePath, string gameName)
        {
            var resDir = Path.Combine(gamePath, gameName + "_Data", "Resources");
            if (Directory.Exists(resDir))
            {
                RegisterFile("Library/unity default resources", new SerializedFile(File.ReadAllBytes(Path.Combine(resDir, "unity default resources"))));
                RegisterFile("Resources/unity_builtin_extra", new SerializedFile(File.ReadAllBytes(Path.Combine(resDir, "unity_builtin_extra"))));
            }
        }
        public static void Reset()
        {
            loadedFiles.Clear();
            loadedBins.Clear();
        }
    }
}
