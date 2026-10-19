using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class ComponentPair : Object
    {
        public PPtr<Component> component = new();
    }
    public partial class GameObject : Object
    {
        public string m_Name = "";
        public Transform? transform { get { return GetComponent<Transform>(); } }
        public List<ComponentPair> m_Component = [];
        public List<Component> GetComponents()
        {
            List<Component> r = [];
            foreach (var o in m_Component)
            {
                if (o.component.TryGet(out var component)) r.Add(component);
            }
            return r;
        }
        public T? GetComponent<T>() where T : Component
        {
            foreach (var o in m_Component)
            {
                if (o.component.TryGet(out var component) && component is T t) return t;
            }
            return null;
        }
        private void CollectComponentsInChildren<T>(GameObject obj, List<T> list) where T : Component
        {
            foreach (var o in obj.m_Component)
            {
                if (o.component.TryGet(out var component) && component is T t) list.Add(t);
            }
            var tf = obj.transform;
            if (tf != null)
            {
                foreach (var c in tf.children)
                {
                    if (c.gameObject != null) CollectComponentsInChildren(c.gameObject, list);
                }
            }
        }
        public List<T> GetComponentsInChildren<T>() where T : Component
        {
            List<T> list = [];
            CollectComponentsInChildren(this, list);
            return list;
        }
    }
}
