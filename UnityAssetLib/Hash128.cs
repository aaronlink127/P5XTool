using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public class Hash128
    {
        public long p1;
        public long p2;
        public override string ToString()
        {
            return $"{p1:X16}{p2:X16}";
        }
    }
}
