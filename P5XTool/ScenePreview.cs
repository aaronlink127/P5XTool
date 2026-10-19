using OpenTK.Mathematics;
using P5XLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAssetLib;
using OpenTK.Graphics.OpenGL4;

namespace P5XTool
{
    public class ScenePreview : IDisposable
    {

        public Scene? scene;
        Matrix4 projection;
        int width;
        int height;
        public float cameraDistance = 1f;
        public Vector3 cameraRot = new(0, (float)Math.PI, 0);
        public Vector3 cameraPos = new(0, 1, 0);
        public float cameraFov = MathF.PI / 2;
        GpuProgram defaultShader = null!;
        SceneMaterial defaultMaterial = null!;
        GpuTexture defaultTexture = null!;
        List<IDisposable> additionalCleanup = [];
        private readonly ObjectCache<GpuTexture, Texture> texCache = new();
        private readonly ObjectCache<SceneMesh, Mesh> meshCache = new();

        public ScenePreview()
        {
            texCache.Load = asset =>
            {
                var tex = GpuUtils.LoadUnityTexture(asset);
                additionalCleanup.Add(tex);
                return tex;
            };
            meshCache.Load = asset =>
            {
                var mesh = GpuUtils.LoadUnityMesh(asset);
                additionalCleanup.Add(mesh);
                return mesh;
            };
        }

        public void ResetCamera()
        {
            cameraRot = new Vector3(0, (float)Math.PI, 0);
            cameraPos = Vector3.Zero;
            cameraDistance = 1f;
        }
        public void Dispose()
        {
            foreach (var d in additionalCleanup)
            {
                d.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public void Init()
        {
            const string vertShaderStr = @"
        #version 430 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aNor;
        layout (location = 2) in vec3 aTan;
        layout (location = 3) in vec3 aCol;
        layout (location = 4) in vec2 aUv;
        layout (location = 5) in vec2 aUv1;
        layout (location = 6) in vec2 aUv2;
        layout (location = 7) in vec2 aUv3;
        layout (location = 8) in vec2 aUv4;
        layout (location = 9) in vec2 aUv5;
        layout (location = 10) in vec2 aUv6;
        layout (location = 11) in vec2 aUv7;
        layout (location = 12) in vec4 aBoneWeight;
        layout (location = 13) in uvec4 aBoneIdx;

        struct vertexOut {
            vec3 normal;
            vec4 color;
            vec2 uv;
        };

        layout(std140, binding = 0) uniform GlobalBlock {
            mat4 mModel;
            mat4 mView;
            mat4 mProjection;
            int mNumBones;
            vec3 sunDirection;
        };

        layout(std430, binding = 1) buffer BoneBuffer {
            mat4 mBones[];
        };

        out vertexOut vs_out;

        void main()
        {
            vec4 skinnedPos = vec4(aPosition, 1.0);
            vec3 skinnedNormal = aNor;
            
            if (mNumBones != 0) {
                skinnedPos =
                    (mBones[aBoneIdx.x] * vec4(aPosition, 1.0)) * aBoneWeight.x +
                    (mBones[aBoneIdx.y] * vec4(aPosition, 1.0)) * aBoneWeight.y +
                    (mBones[aBoneIdx.z] * vec4(aPosition, 1.0)) * aBoneWeight.z +
                    (mBones[aBoneIdx.w] * vec4(aPosition, 1.0)) * aBoneWeight.w;
            
                skinnedNormal =
                    (mat3(mBones[aBoneIdx.x]) * aNor) * aBoneWeight.x +
                    (mat3(mBones[aBoneIdx.y]) * aNor) * aBoneWeight.y +
                    (mat3(mBones[aBoneIdx.z]) * aNor) * aBoneWeight.z +
                    (mat3(mBones[aBoneIdx.w]) * aNor) * aBoneWeight.w;
            }

            mat4 mModelView = mView * mModel;
            gl_Position = mProjection * mModelView * skinnedPos;

            mat3 normalMatrix = transpose(inverse(mat3(mModelView)));
            vs_out.normal = normalize(normalMatrix * skinnedNormal);
            vs_out.uv = aUv;
        }";
            const string fragShaderStr = @"
        #version 330 core
        struct vertexOut {
            vec3 normal;
            vec4 color;
            vec2 uv;
        };

        in vertexOut vs_out;
        out vec4 FragColor;
        uniform sampler2D _MainTex;

        void main()
        {
            vec3 r = vs_out.normal;
            vec2 matcapCoords = r.xy;
            float light = 1 - ((length(r.xy - vec2(0,0.3)) - 0.5) * 0.7);
            //vec4 t = vec4(r.xyz, 1);
            //vec4 t = texture(texture0, (r.xy + 1) / 2);
            vec4 t = texture(_MainTex, vs_out.uv) * light;
            //vec4 t = texture(_MainTex, (r.xy + 1) / 2);
            t.a = 1;
            FragColor = t;
        }";

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.FramebufferSrgb);
            GL.CullFace(TriangleFace.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            defaultShader = new(vertShaderStr, fragShaderStr);
            defaultMaterial = new(defaultShader);
            defaultTexture = new(1, 1, InternalFormat.Rgba8, [255, 0, 255, 255], 1, 0, OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat, OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            defaultMaterial.textures["_MainTex"] = defaultTexture;
            additionalCleanup.Add(defaultTexture);
            additionalCleanup.Add(defaultShader);

            scene = new Scene();
            additionalCleanup.Add(scene);
            ResetCamera();
        }

        private void printTree(SceneObject obj, int depth = 0)
        {
            Console.WriteLine($"{new string(' ', depth)}{obj.Name}");
            foreach (var tf in obj.Transform.Children)
            {
                printTree(tf.Object, depth + 1);
            }
        }

        public void UpdateSize(int width, int height)
        {
            this.width = width;
            this.height = height;
            GL.Viewport(0, 0, width, height);
            UpdateProjection();
        }

        public void UpdateProjection()
        {
            float aspect_ratio = Math.Max(width, 1) / (float)Math.Max(height, 1);
            projection = Matrix4.CreatePerspectiveFieldOfView(cameraFov, aspect_ratio, 0.01f, 100) * Matrix4.CreateScale(-1, 1, 1);
        }

        public void Render()
        {
            if (scene == null) return;
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            GL.ClearColor(0.01f, 0.01f, 0.01f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float cosPitch = MathF.Cos(cameraRot.X);
            float sinPitch = MathF.Sin(cameraRot.X);
            float cosYaw = MathF.Cos(cameraRot.Y);
            float sinYaw = MathF.Sin(cameraRot.Y);

            Vector3 forward = new(
                sinYaw * cosPitch,
                sinPitch,
                cosYaw * cosPitch
            );

            Vector3 cameraWorldPos = cameraPos - forward * cameraDistance;
            Vector3 target = cameraPos;
            Vector3 up = Vector3.UnitY;

            Matrix4 view = Matrix4.LookAt(cameraWorldPos, target, up);

            Matrix4 model = Matrix4.Identity;

            scene.sceneUbo.UpdateMVP(model, view, projection);
            scene.sceneUbo.UpdateSunDirection(forward);
            scene.Render();
            GL.Flush();
        }

        private SceneMesh LoadMeshByName(string assetName)
        {
            var meshAssetR = AssetManager.LoadAsset<Mesh>(assetName);
            if (meshAssetR == null) throw new Exception("Failed to load mesh: " + assetName);
            return LoadMesh(meshAssetR.Asset);
        }
        private SceneMesh LoadMesh(Mesh meshAsset)
        {
            return meshCache.Get(meshAsset);
        }
        private SceneMesh? LoadMesh(PPtr<Mesh> meshPtr)
        {
            return meshCache.Get(meshPtr);
        }

        private SceneMaterial LoadMaterial(Material uMat)
        {
            var mat = new SceneMaterial(defaultShader);
            UnityTexEnv? mainTexProp;
            if (!uMat.m_SavedProperties.m_TexEnvs.TryGetValue("_MainTex", out mainTexProp))
            {
                uMat.m_SavedProperties.m_TexEnvs.TryGetValue("_EyeDiffuseTex", out mainTexProp);
            }
            if (mainTexProp != null)
            {
                var texPtr = mainTexProp.m_Texture;
                var sceneTex = texCache.Get(texPtr);
                mat.textures["_MainTex"] = sceneTex ?? defaultTexture;
            }
            return mat;
        }
        private List<KeyValuePair<SceneMeshRenderer, object[]>> deferredBones = [];
        public SceneObject CreateObjectFromUnity(GameObject gameObject, bool createChildren = true)
        {
            //Console.WriteLine("Creating object: " + gameObject.m_Name);
            var newObj = scene!.CreateObject();
            newObj.Name = gameObject.m_Name;
            newObj.SetId(gameObject.GetHashCode());
            var components = gameObject.GetComponents();
            MeshFilter? meshFilter = null;
            foreach (var component in components)
            {
                switch (component)
                {
                    case Transform tf:
                        {
                            var localPos = tf.m_LocalPosition;
                            var localRot = tf.m_LocalRotation;
                            var localScale = tf.m_LocalScale;
                            var posMtx = Matrix4.CreateTranslation(new(localPos.X, localPos.Y, localPos.Z));
                            var rotMtx = Matrix4.CreateFromQuaternion(new(localRot.X, localRot.Y, localRot.Z, localRot.W));
                            var scaleMtx = Matrix4.CreateScale(new Vector3(localScale.X, localScale.Y, localScale.Z));
                            newObj.Transform.TransformMatrix = scaleMtx * rotMtx * posMtx;
                            newObj.Transform.SetId(tf.GetHashCode());
                            if (createChildren)
                            {
                                foreach (var child in tf.children)
                                {
                                    var subObj = CreateObjectFromUnity(child.gameObject!);
                                    newObj.Transform.AddChild(subObj.Transform);
                                }
                            }
                            break;
                        }
                    case MUActorMeshExportInfo exportInfo:
                        {
                            var mesh = LoadMeshByName(exportInfo.mHighMeshName);
                            var meshrenderer = new SceneMeshRenderer(mesh);
                            var defBones = new object[exportInfo.mHighMeshBoneNames.Length + 1];
                            defBones[0] = exportInfo.mHighRootBoneName;
                            for (int i = 0; i < exportInfo.mHighMeshBoneNames.Length; i++)
                            {
                                defBones[i + 1] = exportInfo.mHighMeshBoneNames[i];
                            }
                            deferredBones.Add(new(meshrenderer, defBones));
                            foreach (var material in exportInfo.mHighMeshMaterials)
                            {
                                if (material.IsNull() || !material.TryGet(out var uMat))
                                {
                                    meshrenderer.materials.Add(defaultMaterial);
                                }
                                else
                                {
                                    var mat = LoadMaterial(uMat);
                                    meshrenderer.materials.Add(mat);
                                }
                            }
                            for (int i = exportInfo.mHighMeshMaterials.Count; i < mesh.subMeshes.Count; i++)
                            {
                                meshrenderer.materials.Add(defaultMaterial);
                            }
                            newObj.AddComponent(meshrenderer);
                            break;
                        }
                    case SkinnedMeshRenderer skinnedMeshRenderer:
                        {
                            SceneMesh? mesh = LoadMesh(skinnedMeshRenderer.m_Mesh);
                            if (mesh == null) break;
                            var meshrenderer = new SceneMeshRenderer(mesh);
                            foreach (var material in skinnedMeshRenderer.m_Materials)
                            {
                                if (material.IsNull() || !material.TryGet(out var uMat))
                                {
                                    meshrenderer.materials.Add(defaultMaterial);
                                }
                                else
                                {
                                    var mat = LoadMaterial(uMat);
                                    meshrenderer.materials.Add(mat);
                                }
                            }
                            var defBones = new object[skinnedMeshRenderer.m_Bones.Count + 1];
                            defBones[0] = skinnedMeshRenderer.m_RootBone.GetObjectHashCode();
                            for (int i = 0; i < skinnedMeshRenderer.m_Bones.Count; i++)
                            {
                                defBones[i + 1] = skinnedMeshRenderer.m_Bones[i].GetObjectHashCode();
                            }
                            deferredBones.Add(new(meshrenderer, defBones));
                            meshrenderer.Enabled = skinnedMeshRenderer.m_Enabled;
                            newObj.AddComponent(meshrenderer);
                            break;
                        }
                    case MeshRenderer meshRenderer:
                        {
                            if (meshFilter == null) break;
                            var mesh = LoadMesh(meshFilter.m_Mesh);
                            if (mesh == null) break;
                            var meshrenderer = new SceneMeshRenderer(mesh);
                            meshrenderer.bones = [];
                            meshrenderer.Enabled = meshRenderer.m_Enabled;
                            meshrenderer.firstSubMesh = meshRenderer.m_StaticBatchInfo.firstSubMesh;
                            meshrenderer.subMeshCount = meshRenderer.m_StaticBatchInfo.subMeshCount;
                            foreach (var material in meshRenderer.m_Materials)
                            {
                                if (material.IsNull() || !material.TryGet(out var uMat))
                                {
                                    meshrenderer.materials.Add(defaultMaterial);
                                }
                                else
                                {
                                    var mat = LoadMaterial(uMat);
                                    meshrenderer.materials.Add(mat);
                                }
                            }
                            newObj.AddComponent(meshrenderer);
                            break;
                        }
                    case MeshFilter mf:
                        {
                            meshFilter = mf;
                            break;
                        }
                }
            }
            return newObj;
        }

        private SceneTransform? resolveBone(object bone)
        {
            if (bone is int boneId)
            {
                var obj = scene!.GetObjectByTransformId(boneId);
                if (obj == null)
                {
                    Console.WriteLine("Bone not found: " + boneId);
                }
                return obj?.Transform;
            }
            else if (bone is string boneName)
            {
                var obj = scene!.GetObjectByName(boneName);
                if (obj == null)
                {
                    Console.WriteLine("Bone not found: " + boneName);
                }
                return obj?.Transform;
            }
            return null;
        }

        public SceneObject AddObject(GameObject obj)
        {
            deferredBones = [];
            var rootObj = CreateObjectFromUnity(obj);
            foreach (var kvp in deferredBones)
            {
                var meshrenderer = kvp.Key;
                //Console.WriteLine("Loading bones for " + meshrenderer.SceneObject.Name);
                var defBones = kvp.Value;
                var bones = new SceneTransform[defBones.Length - 1];
                meshrenderer.rootBone = resolveBone(defBones[0]);
                for (int i = 0; i < bones.Length; i++)
                {
                    bones[i] = resolveBone(defBones[i + 1])!;
                }
                //Console.WriteLine($"Loaded {bones.Length} bones (bindPose {meshrenderer._mesh.bindPose.Length})");
                meshrenderer.bones = bones;
            }
            return rootObj;
        }

        public SceneObject AddMesh(Mesh umesh)
        {
            var mesh = LoadMesh(umesh);
            var obj = new SceneObject(scene!);
            var meshrenderer = new SceneMeshRenderer(mesh);
            obj.AddComponent(meshrenderer);
            for (int i = 0; i < mesh.subMeshes.Count; i++)
            {
                meshrenderer.materials.Add(defaultMaterial);
            }
            obj.Name = umesh.m_Name;
            return obj;
        }

        public SceneObject? CreatePrefab(string prefabName)
        {
            Console.WriteLine("Creating prefab: " + prefabName);
            var prefabAssetR = AssetManager.LoadAsset<GameObject>(prefabName);
            if (prefabAssetR == null) return null;
            return AddObject(prefabAssetR.Asset);
        }
        public void LoadScene(string unityFile)
        {
            var bundleName = AssetBundleUtils.GetBundleByAsset(unityFile);
            if (bundleName == null) return;
            var bundle = AssetManager.LoadBundle(bundleName);
            Dictionary<int, int> parentMap = [];
            var sf = bundle.Bundle.SerializedFiles[1];
            foreach (var kv in sf.GetObjectInfos())
            {
                if (sf.GetObjectTypeName(kv.Key) == "GameObject")
                {
                    if (sf.GetObject(kv.Key) is not GameObject obj) continue;
                    var tf = obj.transform!;
                    if (!tf.m_Father.IsNull())
                    {
                        int tfId = tf.GetHashCode();
                        int tfParentId = tf.m_Father.GetObjectHashCode();
                        parentMap.Add(tfId, tfParentId);
                    }
                    var sceneObj = CreateObjectFromUnity(obj, false);
                }
            }
            foreach (var kv in parentMap)
            {
                var childObj = scene!.GetObjectByTransformId(kv.Key);
                var parentObj = scene.GetObjectByTransformId(kv.Value);
                if (childObj != null && parentObj != null)
                {
                    childObj.Transform.Parent = parentObj.Transform;
                }
            }
        }

    }

    public class ObjectCache<J, T> where T : UnityAssetLib.Object where J : class
    {
        private Dictionary<int, J> cache = [];
        public Func<T, J> Load = null!;

        public J Get(T asset)
        {
            int hash = asset.GetHashCode();
            if (cache.TryGetValue(hash, out var cached))
            {
                return cached;
            }
            var obj = Load(asset);
            cache[hash] = obj;
            return obj;
        }
        public J? Get(PPtr<T> ptr)
        {
            if (ptr.IsNull())
            {
                return null;
            }
            int hash = ptr.GetObjectHashCode();
            if (cache.TryGetValue(hash, out var cached))
            {
                return cached;
            }
            if (ptr.TryGet(out var asset))
            {
                var obj = Load(asset);
                cache[hash] = obj;
                return obj;
            }
            return null;
        }
    }
}
