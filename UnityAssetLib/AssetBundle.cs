using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class AssetBundle : Object
    {
        [AutoIgnore]
        public List<string> cabNames = [];
        public List<SerializedFile> SerializedFiles
        {
            get
            {
                return UnityAssetManager.GetFilesInBundle(this);
            }
        }

        public string m_Name = "";
        public string[] m_Dependencies = [];
        public bool m_IsStreamedSceneAssetBundle;
        public AssetInfo m_MainAsset = new();
        public List<KeyValuePair<string, AssetInfo>> m_Container = [];
        public T? LoadAsset<T>(string path) where T : Object, new()
        {
            foreach (var kvp in m_Container)
            {
                if (string.Equals(kvp.Key, path, StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value.asset.TryGet(out Object? result))
                    {
                        if (result is T r)
                        {
                            return r;
                        }
                        else
                        {
                            Console.WriteLine($"Found asset, wrong type? expected: {typeof(T).Name} got: {result.GetType().Name}");
                        }
                    }
                }
            }
            return null;
        }
        public Object? LoadAsset(string path)
        {
            foreach (var kvp in m_Container)
            {
                if (string.Equals(kvp.Key, path, StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value.asset.TryGet(out Object? result))
                    {
                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get asset {path}");
                    }
                }
            }
            return null;
        }

        public static AssetBundle LoadFromMemory(byte[] data)
        {
            return UnityAssetManager.LoadAssetBundle(data);
        }

        public void Unload()
        {
            foreach (var cabName in cabNames)
            {
                UnityAssetManager.UnloadArchive(cabName);
            }
        }
    }
}
