using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class AutoIgnoreAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AutoOptionalAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AutoFieldNameAttribute : Attribute
    {
        public string Name { get; }
        public AutoFieldNameAttribute(string name) => Name = name;
    }
}
