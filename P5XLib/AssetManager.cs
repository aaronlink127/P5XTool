using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public class AssetManager
    {
        static readonly Dictionary<string, WeakReference<BundleRef>> _allBundleRefs = [];
        private static readonly List<BundleRef> _keepLoadedBundles = [];
        public static void Init()
        {
            CachedBundle("program_mat.bundle");
        }
        private static void CachedBundle(string bundleName)
        {
            _keepLoadedBundles.Add(LoadBundle(bundleName, false));
        }
        public static void UnloadAll()
        {
            foreach (var bundleRefWeak in _allBundleRefs.Values)
            {
                if (bundleRefWeak.TryGetTarget(out var bundleRef))
                {
                    bundleRef.Dispose();
                }
            }
            _keepLoadedBundles.Clear();
            _allBundleRefs.Clear();
        }
        public static void Reset()
        {
            UnloadAll();
        }
        public static BundleRef NewRef(string abName)
        {
            var bundleRef = new BundleRef(abName);
            _allBundleRefs[abName] = new(bundleRef);
            return bundleRef;
        }
        private static BundleRef? GetRef(string abName)
        {
            if (_allBundleRefs.TryGetValue(abName, out var bundleRefWeak) && bundleRefWeak.TryGetTarget(out var bundleRef))
            {
                return bundleRef;
            }
            return null;
        }
        public static BundleRef LoadSingleBundle(string abName)
        {
            return GetRef(abName) ?? NewRef(abName);
        }
        public static BundleRef LoadBundle(string abName, bool needDependencies = true)
        {
            var bundle = GetRef(abName);
            if (bundle == null)
            {
                bundle = LoadSingleBundle(abName);
                var deps = AssetBundleUtils.GetDirectDependencies(abName);
                List<BundleRef> bundles = [];
                foreach (var dep in deps)
                {
                    bundles.Add(LoadBundle(dep));
                }
                bundle.SetDependencies(bundles);
            }
            if (needDependencies)
            {
                bundle.SetNeedDependencies();
            }
            return bundle;
        }
        public static BundleAssetRef<T>? LoadAsset<T>(string name) where T : UnityAssetLib.Object, new()
        {
            var bundle = AssetBundleUtils.GetBundleByAsset(name);
            if (bundle == null) return null;
            var abR = LoadBundle(bundle);
            return abR.LoadAsset<T>(name);
        }
        public static BundleAssetRef? LoadAsset(string name)
        {
            var bundle = AssetBundleUtils.GetBundleByAsset(name);
            if (bundle == null) return null;
            var abR = LoadBundle(bundle);
            return abR.LoadAsset(name);
        }
        public static string? LoadText(string name)
        {
            var assetRef = LoadAsset<TextAsset>(name);
            if (assetRef == null) return null;
            return Encoding.UTF8.GetString(assetRef.Asset.m_Script);
        }
        public static byte[]? LoadBytes(string name)
        {
            var assetRef = LoadAsset<TextAsset>(name);
            if (assetRef == null) return null;
            return assetRef?.Asset.m_Script;
        }
    }
}
