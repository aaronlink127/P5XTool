using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class Transform : Component
    {
        public Quaternion m_LocalRotation;
        public Vector3 m_LocalPosition;
        public Vector3 m_LocalScale;
        public List<PPtr<Transform>> m_Children = [];
        public PPtr<Transform> m_Father = new();
        public IEnumerable<Transform> children { get
            {
                foreach (var childPtr in m_Children)
                {
                    if (childPtr.TryGet(out var child))
                    {
                        yield return child;
                    }
                }
            } }
        //public override void LoadDataFrom(Dictionary<string, object?> data, AssetBundle _sourceAssetBundle)
        //{
        //    base.LoadDataFrom(data, _sourceAssetBundle);
        //    var localRot = (Dictionary<string, object?>)data["m_LocalRotation"];
        //    var localPos = (Dictionary<string, object?>)data["m_LocalPosition"];
        //    var localScale = (Dictionary<string, object?>)data["m_LocalScale"];
        //    m_LocalRotation = new((float)localRot["x"], (float)localRot["y"], (float)localRot["z"], (float)localRot["w"]);
        //    m_LocalPosition = new((float)localPos["x"], (float)localPos["y"], (float)localPos["z"]);
        //    m_LocalScale = new((float)localScale["x"], (float)localScale["y"], (float)localScale["z"]);
        //    m_Children = [];
        //    foreach (var childO in (object[])(data["m_Children"]))
        //    {
        //        var ptr = new PPtr<Transform>();
        //        ptr.LoadDataFrom(childO, _sourceAssetBundle);
        //        m_Children.Add(ptr);
        //    }
        //}
    }
}
