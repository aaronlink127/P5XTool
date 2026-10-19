using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnityAssetLib
{
    public partial class MonoBehaviour : Component
    {
        public PPtr<MonoScript> m_Script = new();
    }
}
