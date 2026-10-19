using OpenTK.Graphics.OpenGL4;
using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace P5XTool
{
    public class Scene : IDisposable
    {
        private readonly List<SceneObject> objects = [];
        public SceneGlobals sceneUbo = new();
        //private SceneMesh testMesh;
        public Scene()
        {
        }
        public void Render()
        {
            foreach (SceneObject obj in objects)
            {
                obj.Render();
            }
        }
        public void AddObject(SceneObject obj)
        {
            objects.Add(obj);
        }
        public void RemoveObject(SceneObject obj)
        {
            objects.Remove(obj);
        }
        public SceneObject CreateObject()
        {
            return new(this);
        }
        public SceneObject? GetObjectByName(string name)
        {
            return objects.FirstOrDefault(x => x.Name == name);
        }
        public SceneObject? GetObjectById(long id)
        {
            return objects.FirstOrDefault(x => x.Id == id);
        }
        public SceneObject? GetObjectByTransformId(int id)
        {
            return objects.FirstOrDefault(x => x.Transform.Id == id);
        }
        public List<SceneObject> GetRootObjects()
        {
            return [.. objects.Where(o => o.Transform.Parent == null)];
        }
        public List<SceneObject> GetAllObjects()
        {
            return [.. objects];
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            sceneUbo.Dispose();
        }
        ~Scene()
        {
            Dispose();
        }
    }
}
