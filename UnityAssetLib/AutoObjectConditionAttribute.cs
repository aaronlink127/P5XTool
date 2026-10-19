using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoObjectConditionAttribute : Attribute
    {
    }

    public class UnityMinVersionAttribute : AutoObjectConditionAttribute
    {
        public readonly UnityVersion version;
        public UnityMinVersionAttribute(int major)
        {
            version = new(major);
        }
        public UnityMinVersionAttribute(int major, int minor, int patch)
        {
            version = new(major, minor, patch);
        }
        public static bool Evaluate(Dictionary<string, object?> _, SerializedFile sf, int major)
        {
            return sf.unityVersion >= new UnityVersion(major, 0, 0);
        }
    }
}
