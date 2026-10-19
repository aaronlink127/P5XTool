using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public class AssetUtils
    {
        public static uint GetHashCode(string s)
        {
            uint n = 0;
            foreach (char c in s)
            {
                n = c + n * 131;
            }
            return n & 0x7FFFFFFF;
        }
        public static int GetBundleObfuscateOffset(string s)
        {
            if (GameMgr.IsCbt) return 0;
            int n = 0;
            foreach (char c in s)
            {
                n = c + n * 131;
            }
            return (n & 0x1F) + 8;
        }
    }
}
