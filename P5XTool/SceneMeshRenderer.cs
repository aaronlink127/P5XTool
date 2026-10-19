using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P5XTool
{
    public class SceneMeshRenderer : SceneComponent
    {
        public bool Enabled = true;
        public SceneMesh _mesh;
        public SceneTransform[] bones;
        public SceneTransform? rootBone;
        public List<SceneMaterial> materials = [];
        public int firstSubMesh = 0;
        public int subMeshCount = 0;
        public SceneMeshRenderer(SceneMesh mesh) {
            _mesh = mesh;
            bones = [];
        }
        public override void Render()
        {
            if (!Enabled) return;

            Matrix4 model;
            SceneTransform rootTf = rootBone ?? SceneObject.Transform;
            model = rootTf.GetFullTransform();

            //const int MaxMatrices = 256;
            //const int FloatsPerMat = 16;
            //float[] flatFloat = new float[bones.Length * FloatsPerMat];

            //int idx = 0;
            //for (int i = 0; i < bones.Length; i++)
            //{
            //    var tf = Matrix4.Identity;
            //    if (bones[i] != null)
            //    {
            //        tf = bones[i].GetTransformFrom(rootBone);
            //    }
            //    var skin = _mesh.bindPose[i] * tf;

            //    flatFloat[idx++] = skin.M11;
            //    flatFloat[idx++] = skin.M12;
            //    flatFloat[idx++] = skin.M13;
            //    flatFloat[idx++] = skin.M14;
                
            //    flatFloat[idx++] = skin.M21;
            //    flatFloat[idx++] = skin.M22;
            //    flatFloat[idx++] = skin.M23;
            //    flatFloat[idx++] = skin.M24;
                
            //    flatFloat[idx++] = skin.M31;
            //    flatFloat[idx++] = skin.M32;
            //    flatFloat[idx++] = skin.M33;
            //    flatFloat[idx++] = skin.M34;
                
            //    flatFloat[idx++] = skin.M41;
            //    flatFloat[idx++] = skin.M42;
            //    flatFloat[idx++] = skin.M43;
            //    flatFloat[idx++] = skin.M44;
            //}
            int boneCount = _mesh.bindPose.Length;
            Matrix4[] boneMatrices = new Matrix4[_mesh.bindPose.Length];
            for (int i = 0; i < boneCount; i++)
            {
                var tf = Matrix4.Identity;
                if (i < bones.Length && bones[i] != null)
                {
                    tf = _mesh.bindPose[i] * bones[i].GetTransformFrom(rootTf);
                }
                boneMatrices[i] = tf;
            }
            _mesh.Bind();

            var sceneUbo = SceneObject.Scene.sceneUbo;
            sceneUbo.UpdateBones(boneMatrices);
            int matIndex = 0;
            int submeshCount = subMeshCount > 0 ? subMeshCount : _mesh.subMeshes.Count;
            for (int i = firstSubMesh; i < firstSubMesh + submeshCount; i++)
            {
                var submesh = _mesh.subMeshes[i];
                var material = materials[matIndex];
                material.Bind();
                sceneUbo.UpdateModel(model);
                sceneUbo.Bind();
                GL.DrawElements(submesh.BeginMode, (int)submesh.indexCount, _mesh.elementsType, (int)submesh.firstByte);
                matIndex++;
            }
        }
    }
}
