using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XTool
{
    public abstract class SceneComponent
    {
        private SceneObject? _sceneObject;
        public SceneObject SceneObject => _sceneObject!;
        public virtual void Render()
        {

        }
        public void SetSceneObject(SceneObject sceneObject)
        {
            _sceneObject = sceneObject;
        }
    }
}
