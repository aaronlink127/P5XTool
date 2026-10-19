using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class StaticBatchInfo : Object
    {
        public ushort firstSubMesh;
        public ushort subMeshCount;
    }
    public partial class MeshRenderer : Renderer
    {
        public StaticBatchInfo m_StaticBatchInfo = new();
    }
}
