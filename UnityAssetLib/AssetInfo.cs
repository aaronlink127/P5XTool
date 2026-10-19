using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class AssetInfo : Object
    {
        public int preloadIndex;
        public int preloadSize;
        public PPtr<Object> asset = new();
    }
}
