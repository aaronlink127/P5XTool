using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class SkinnedMeshRenderer : Renderer
    {
        public PPtr<Mesh> m_Mesh = new();
        public PPtr<Transform> m_RootBone = new();
        public List<PPtr<Transform>> m_Bones = [];
        public float[] m_BlendShapeWeights = [];
    }
}
