using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAssetLib;

namespace P5XLib
{
    public class BundleAssetRef(BundleRef _r, UnityAssetLib.Object _o)
    {
        public UnityAssetLib.Object Asset => _o;
        public BundleRef BundleRef => _r;
        public T As<T>() where T : class
        {
            return _o as T ?? throw new InvalidOperationException();
        }
    }

    public class BundleAssetRef<T>(BundleAssetRef assetRef) where T : UnityAssetLib.Object
    {
        public T Asset => assetRef.As<T>();
    }
}
