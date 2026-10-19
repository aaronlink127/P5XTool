using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class MeshFilter : Component
    {
        public PPtr<Mesh> m_Mesh = new();
    }
}
