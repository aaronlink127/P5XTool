using P5XLib;
using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zeus.Framework.Asset;
using Google.FlatBuffers;

namespace P5XLib
{
    public static class AssetBundleUtils
    {
        static string AssetMapNameFb => GameMgr.IsCbt ? "assetMapName.fb" : "assetMapName.txt";
        static string UnloadbleBundlesFb => GameMgr.IsCbt ? "unloadbleBundles.fb" : "unloadableBundles.txt";
        static string ZeusBundleManifestFb => GameMgr.IsCbt ? "zeusBundleManifest.fb" : "zeusBundleManifest.txt";
        static public bool IsInit { get; private set; }
        static Dictionary<uint, string> _assetHashMapBundles = [];
        static Dictionary<string, string> _assetPathMapBundles = [];
        static Dictionary<string, List<string>> _bundle2Dependencies = [];
        static Dictionary<string, List<string>> _dependencies2Bundles = [];
        static HashSet<string> _unloadableBundles = [];
        static void LoadAssetMapFB()
        {
            var assetMapBytes = VFileSystem.ReadAllBytes(GetRelativeBundlePath(AssetMapNameFb));

            var bb = new ByteBuffer(assetMapBytes);
            var assetMap = AssetMapBundleFB.GetRootAsAssetMapBundleFB(bb);
            for (int i = 0; i < assetMap.H2bLength; i++)
            {
                var p2b = assetMap.H2b(i)!.Value;
                _assetHashMapBundles[p2b.Hash!] = p2b.Bundle!;
            }
            for (int i = 0; i < assetMap.P2bLength; i++)
            {
                var p2b = assetMap.P2b(i)!.Value;
                _assetPathMapBundles[p2b.Path!] = p2b.Bundle!;
            }
        }
        static void LoadManifestFB()
        {
            var zeusManifestBytes = VFileSystem.ReadAllBytes(GetRelativeBundlePath(ZeusBundleManifestFb));
            var bb = new ByteBuffer(zeusManifestBytes);
            var manifest = ZeusBundleManifestFB.GetRootAsZeusBundleManifestFB(bb);
            for (int i = 0; i < manifest.InfosLength; i++)
            {
                var bundleInfo = manifest.Infos(i)!.Value;
                var bundleName = bundleInfo.Name;
                List<string> deps = [];
                for (int j = 0; j < bundleInfo.DependenciesLength; j++)
                {
                    var depIdx = bundleInfo.Dependencies(j)!;
                    var depName = manifest.Infos(depIdx)!.Value.Name;
                    deps.Add(depName);
                    var reverseDeps = _dependencies2Bundles.GetValueOrDefault(depName, []);
                    reverseDeps.Add(bundleName);
                    _dependencies2Bundles[depName] = reverseDeps;
                }
                _bundle2Dependencies.Add(bundleName, deps);
            }
        }
        static void LoadUnloadablesFB()
        {
            _unloadableBundles = [];
            var unloadableBundleBytes = VFileSystem.ReadAllBytes(GetRelativeBundlePath(UnloadbleBundlesFb));
            var bb = new ByteBuffer(unloadableBundleBytes);
            var unloadables = UnloadbleAssetBundle.GetRootAsUnloadbleAssetBundle(bb);
            for (int i = 0; i < unloadables.BundlesLength; i++)
            {
                var bundleName = unloadables.Bundles(i);
                if (bundleName != null)
                {
                    _unloadableBundles.Add(bundleName);
                }
            }
        }
        public static List<string> GetDirectDependencies(string bundleName)
        {
            if (_bundle2Dependencies.TryGetValue(bundleName, out List<string>? deps))
            {
                return deps;
            }
            return [];
        }
        public static List<string> GetDependants(string bundleName)
        {
            if (_dependencies2Bundles.TryGetValue(bundleName, out List<string>? deps))
            {
                return deps;
            }
            return [];
        }
        public static void GetAllDependencies(string bundleName, HashSet<string> collectedDeps)
        {
            if (_bundle2Dependencies.TryGetValue(bundleName, out List<string>? deps))
            {
                foreach (string dep in deps)
                {
                    if (!collectedDeps.Contains(dep))
                    {
                        collectedDeps.Add(dep);
                        GetAllDependencies(dep, collectedDeps);
                    }
                }
            }
        }
        public static HashSet<string> GetAllDependencies(string bundleName)
        {
            HashSet<string> deps = [];
            GetAllDependencies(bundleName, deps);
            return deps;
        }
        public static void Init()
        {
            if (IsInit) return;
            if (!VFileSystem.IsInit) return;
            LoadAssetMapFB();
            LoadManifestFB();
            LoadUnloadablesFB();
            IsInit = true;

        }
        public static void Reset()
        {
            IsInit = false;
            _assetHashMapBundles.Clear();
            _assetPathMapBundles.Clear();
            _bundle2Dependencies.Clear();
            _dependencies2Bundles.Clear();
        }
        public static List<string> GetBundles()
        {
            return [.. _bundle2Dependencies.Keys];
        }
        public static bool IsBundleInManifest(string abName)
        {
            return _bundle2Dependencies.ContainsKey(abName);
        }
        public static string? GetBundleByAsset(string assetPath)
        {
            if (_assetHashMapBundles.TryGetValue(AssetUtils.GetHashCode(assetPath), out string? bundleName))
            {
                return bundleName;
            }
            if (_assetPathMapBundles.TryGetValue(assetPath, out bundleName))
            {
                return bundleName;
            }
            return null;
        }
        public static string GetRelativeBundlePath(string abName)
        {
            if (GameMgr.IsCbt)
            {
                return $"Bundles/Windows/{abName}";
            }
            return $"Bundles/Windows/{AssetUtils.GetHashCode(abName) % 200:D3}/{abName}";
        }
    }
}
