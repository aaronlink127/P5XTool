using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class StreamingInfo : Object
    {
        public ulong offset;
        public uint size;
        public string path = "";
        public ReadOnlySpan<byte> GetData()
        {
            return UnityAssetManager.LoadStreamedData(this);
        }
    }
}
