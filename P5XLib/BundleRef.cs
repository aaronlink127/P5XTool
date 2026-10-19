using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace P5XLib
{
    public class BundleRef : IDisposable
    {
        private static readonly Dictionary<string, int> refCounts = [];
        private static readonly Dictionary<string, AssetBundle> bundles = [];
        private AssetBundle? _bundle;
        private List<BundleRef> _dependencies = [];
        private readonly Dictionary<string, BundleAssetRef> _assetRefs = [];
        public string abName;
        private bool needDependencies = false;

        public AssetBundle Bundle
        {
            get
            {
                EnsureLoaded();
                return _bundle!;
            }
        }
        private static AssetBundle LoadBundle(string name)
        {
            var bundleConts = VFileSystem.ReadAllBytes(AssetBundleUtils.GetRelativeBundlePath(name));
            var bundleOffset = AssetUtils.GetBundleObfuscateOffset(name);
            if (BitConverter.ToUInt32(bundleConts, bundleOffset) != 0x74696E55) // "Unit"
            {
                bundleOffset = 0;
                Console.WriteLine($"[Warning] Bundle {name} does not have expected obfuscation offset, loading from start.");
            }
            var aB = AssetBundle.LoadFromMemory(bundleConts.AsSpan(bundleOffset).ToArray());
            return aB;
        }
        private static AssetBundle GetBundle(string name)
        {
            int refCount = refCounts.GetValueOrDefault(name, 0);
            refCounts[name] = refCount+1;
            if (refCount == 0)
            {
                var bundle = LoadBundle(name);
                bundles[name] = bundle;
                return bundle;
            }
            return bundles[name];
        }
        private static void FreeBundle(string name)
        {
            if (--refCounts[name] == 0)
            {
                bundles[name].Unload();
                bundles.Remove(name);
            }
        }
        public BundleRef(string _abName)
        {
            abName = _abName;
            //IncRef();
        }
        public void SetDependencies(List<BundleRef> _deps)
        {
            _dependencies = _deps;
        }
        private void IncRef()
        {
            lock (bundles)
            {
                if (_bundle != null) return;
                _bundle = GetBundle(abName);
            }
        }
        private void DecRef()
        {
            lock (bundles)
            {
                if (_bundle == null) return;
                var bundle = bundles.GetValueOrDefault(abName);
                if (bundle != _bundle)
                {
                    Console.WriteLine($"[Warning] BundleRef for {abName} has mismatched bundle instance during DecRef.");
                    return;
                }
                FreeBundle(abName);
                _bundle = null;
            }
        }

        public void SetNeedDependencies()
        {
            needDependencies = true;
        }
        private void EnsureLoaded(HashSet<string>? checkedBundles = null)
        {
            if (_bundle != null) return;
            if (checkedBundles == null) checkedBundles = [];
            if (checkedBundles.Contains(abName)) return;
            checkedBundles.Add(abName);
            IncRef();
            if (!needDependencies) return;
            foreach (var dep in _dependencies)
            {
                dep.needDependencies = true;
                dep.EnsureLoaded(checkedBundles);
            }
        }
        public void EnsureLoaded()
        {
            EnsureLoaded([]);
        }

        public BundleAssetRef<T>? LoadAsset<T>(string assetName) where T : UnityAssetLib.Object, new()
        {
            var baR = LoadAsset(assetName);
            if (baR == null || baR.Asset is not T) return null;
            return new(baR);
        }
        public BundleAssetRef? LoadAsset(string assetName)
        {
            if (_assetRefs.TryGetValue(assetName, out var assetRef))
            {
                return assetRef;
            }
            var asset = Bundle.LoadAsset(assetName);
            if (asset == null) return null;
            var baR = new BundleAssetRef(this, asset);
            _assetRefs[assetName] = baR;
            return baR;
        }

        public void Dispose()
        {
            _assetRefs.Clear();
            DecRef();
            GC.SuppressFinalize(this);
        }

        ~BundleRef()
        {
            Dispose();
        }
    }
}
