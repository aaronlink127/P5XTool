using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class Component : Object
    {
        public PPtr<GameObject> m_GameObject = new();
        public GameObject? gameObject { get {
            if (m_GameObject.TryGet(out var res)) return res;
            return null;
        } }
    }
}
