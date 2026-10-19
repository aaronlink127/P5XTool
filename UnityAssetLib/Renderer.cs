using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class Renderer : Component
    {
        public bool m_Enabled;
        public List<PPtr<Material>> m_Materials = new();
    }
}
