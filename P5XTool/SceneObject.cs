using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XTool
{
    public class SceneObject
    {
        public string Name = "";
        public bool Active = true;
        private int id;
        public int Id => id;
        private Scene _scene;
        public SceneTransform Transform;
        private readonly List<SceneComponent> _components = [];
        public IReadOnlyList<SceneComponent> Components => _components.AsReadOnly();
        public Scene Scene => _scene;
        public void SetId(int newId)
        {
            id = newId;
        }
        public SceneObject? GetChildByName(string name)
        {
            var tf = Transform;
            var child = tf.GetChildByName(name);
            if (child != null)
            {
                return child.Object;
            }
            return null;
        }
        public SceneObject(Scene scene) {
            _scene = scene;
            scene.AddObject(this);
            Transform = new(this);
        }
        public void AddComponent(SceneComponent obj)
        {
            _components.Add(obj);
            obj.SetSceneObject(this);
        }
        public void Render()
        {
            if (!Active) return;
            foreach (var component in _components)
            {
                component.Render();
            }
        }
    }
}
