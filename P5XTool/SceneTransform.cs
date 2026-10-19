using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace P5XTool
{
    public class SceneTransform
    {
        private int id;
        public int Id => id;
        public Matrix4 TransformMatrix = Matrix4.Identity;
        public SceneTransform? CopyTransform;
        //public Vector3 Translation;
        //public Quaternion Rotation;
        //public Vector3 Scale;
        public void SetId(int newId)
        {
            id = newId;
        }
        public SceneObject Object;
        public SceneTransform(SceneObject obj)
        {
            Object = obj;
        }
        readonly List<SceneTransform> m_Children = [];
        public ReadOnlyCollection<SceneTransform> Children => m_Children.AsReadOnly();
        SceneTransform? m_Parent;
        public SceneTransform? Parent {
            get => m_Parent;
            set => SetParent(value);
        }
        public Vector3 Translation
        {
            get
            {
                return TransformMatrix.ExtractTranslation();
            }
            set
            {
                TransformMatrix = TransformMatrix.ClearTranslation() * Matrix4.CreateTranslation(value);
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return TransformMatrix.ExtractRotation();
            }
            set
            {
                TransformMatrix = TransformMatrix.ClearRotation() * Matrix4.CreateFromQuaternion(value);
            }
        }
        public Vector3 Scale
        {
            get
            {
                return TransformMatrix.ExtractScale();
            }
            set
            {
                TransformMatrix = Matrix4.CreateScale(value) * TransformMatrix.ClearScale();
            }
        }
        public void SetSRT(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            TransformMatrix = Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(translation);
        }
        public Matrix4 GetFullTransform()
        {
            if (CopyTransform != null)
            {
                return CopyTransform.GetFullTransform();
            }
            if (m_Parent == null) return TransformMatrix;
            return TransformMatrix * m_Parent.GetFullTransform();
        }
        public Vector3 GetWorldPosition() {
            var fullTransform = GetFullTransform();
            return fullTransform.ExtractTranslation();
        }
        public Matrix4 GetTransformFrom(SceneTransform tf)
        {
            //if (tf == null) return GetFullTransform();
            if (this == tf) return Matrix4.Identity;
            //return TransformMatrix * m_Parent.GetTransformFrom(tf);
            return GetFullTransform() * tf.GetFullTransform().Inverted();
        }
        public Box3 GetExtent()
        {
            Box3 extent = new();
            CalcExtent(extent);
            return extent;
        }
        private void CalcExtent(Box3 extent)
        {
            var worldPos = GetWorldPosition();
            if (worldPos.X < extent.Min.X && worldPos.Y < extent.Min.Y && worldPos.Z < extent.Min.Z)
            {
                extent.Min = worldPos;
            }
            if (worldPos.X > extent.Max.X && worldPos.Y > extent.Max.Y && worldPos.Z > extent.Max.Z)
            {
                extent.Max = worldPos;
            }
            foreach (var child in m_Children)
            {
                child.CalcExtent(extent);
            }
        }

        //public void SetSRT(Vector3 scale, Quaternion rotation, Vector3 translation)
        //{
        //    Scale = scale;
        //    Rotation = rotation;
        //    Translation = translation;
        //}
        //public Matrix4 GetFullTransform()
        //{
        //    var TransformMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Translation);
        //    if (m_Parent == null) return TransformMatrix;
        //    return m_Parent.GetFullTransform() * TransformMatrix;
        //}
        public SceneTransform? GetChildByName(string name)
        {
            foreach (var child in m_Children)
            {
                if (child.Object.Name == name)
                {
                    return child;
                }
            }
            foreach (var child in m_Children)
            {
                var tf = child.GetChildByName(name);
                if (tf != null) return tf;
            }
            return null;
        }
        public List<SceneTransform> GetChildrenRecursively()
        {
            List<SceneTransform> result = [];
            foreach (var child in m_Children)
            {
                result.Add(child);
                result.AddRange(child.GetChildrenRecursively());
            }
            return result;
        }
        public void SetParent(SceneTransform? parent)
        {
            if (m_Parent == parent) return;
            m_Parent?.m_Children.Remove(this);
            m_Parent = parent;
            parent?.m_Children.Add(this);
        }

        public void AddChild(SceneTransform child)
        {
            child.SetParent(this);
        }
        public void RemoveChild(SceneTransform child)
        {
            if (child.m_Parent == this)
                child.SetParent(null);
        }
    }
}
