using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class UnityTexEnv : Object
    {
        public PPtr<Texture> m_Texture = new();
        public Vector2 m_Scale;
        public Vector2 m_Offset;
    }
    public partial class UnityPropertySheet : Object
    {
        public Dictionary<string, UnityTexEnv> m_TexEnvs = [];
        [UnityMinVersion(2021)]
        public Dictionary<string, int> m_Ints = [];
        public Dictionary<string, float> m_Floats = [];
        public Dictionary<string, Color> m_Colors = [];
    }
    public partial class Material : Object
    {
        public string m_Name = "";
        public UnityPropertySheet m_SavedProperties = new();
    }
}
