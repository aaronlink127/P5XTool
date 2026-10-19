using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace P5XTool
{
    public class SceneSubMesh
    {
        public uint firstByte;
        public uint indexCount;
        public BeginMode BeginMode = BeginMode.Triangles;
    }
    public class SceneMesh : IDisposable
    {
        enum VertexAttribute
        {
            Position = 0,
            Normal = 1,
            Tangent = 2,
            Color = 3,
            TexCoord0 = 4,
            TexCoord1 = 5,
            TexCoord2 = 6,
            TexCoord3 = 7,
            TexCoord4 = 8,
            TexCoord5 = 9,
            TexCoord6 = 10,
            TexCoord7 = 11,
            BlendWeight = 12,
            BlendIndices = 13,
            Count
        }
        public class VertexAttributeDesc
        {
            public int offset;
            public VertexAttribPointerType format;
            public int stride;
            public int dimension;
            public VertexAttributeDesc() { }
            public VertexAttributeDesc(int _offset, VertexAttribPointerType _format, int _stride, int _dimension)
            {
                offset = _offset;
                format = _format;
                stride = _stride;
                dimension = _dimension;
            }
            public override string ToString()
            {
                return $"Offset: {offset}, Format: {format}, Stride: {stride}, Dimension: {dimension}";
            }
        }
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct Vertex
        //{
        //    public static int SizeInBytes => Unsafe.SizeOf<Vertex>();
        //    public Vector3 Position;
        //    public Vector3 Normal;
        //    public Vector2 Uv;
        //    public Vector4i BoneIndices;
        //    public Vector4 BoneWeights;
        //}

        public Matrix4[] bindPose = [];

        public List<SceneSubMesh> subMeshes = [];

        public DrawElementsType elementsType = DrawElementsType.UnsignedInt;

        private int VAO;
        private int VBO;
        private int EBO;

        public VertexAttributeDesc[] vertexAttributes = new VertexAttributeDesc[(int)VertexAttribute.Count];
        //public VertexAttributeDesc positionAttr;
        //public VertexAttributeDesc normalAttr;
        //public VertexAttributeDesc tangentAttr;
        //public VertexAttributeDesc uvAttr;
        //public VertexAttributeDesc boneIndsAttr;
        //public VertexAttributeDesc boneWeightsAttr;


        public SceneMesh()
        {
        }

        public void InitBuffers()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //if (vertices != null && vertices.Length > 0)
            //{
            //    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            //}
            //else
            //{
                GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.StaticDraw);
            //}

            for (int i = 0; i < vertexAttributes.Length; i++) {
                var attr = vertexAttributes[i];
                bool isInt = Enum.IsDefined(typeof(VertexAttribIntegerType), (int)attr.format);
                if (attr.dimension == 0)
                {
                    GL.DisableVertexAttribArray(i);
                    if (isInt)
                        GL.VertexAttrib4(i, 0, 0, 0, 0);
                    else
                        GL.VertexAttrib4(i, 1.0f, 0.0f, 0.0f, 0.0f);
                }
                else
                {
                    GL.EnableVertexAttribArray(i);
                    if (isInt)
                        GL.VertexAttribIPointer(i, attr.dimension, (VertexAttribIntegerType)attr.format, attr.stride, attr.offset);
                    else
                        GL.VertexAttribPointer(i, attr.dimension, attr.format, false, attr.stride, attr.offset);
                }
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            //if (indices != null && indices.Length > 0)
            //{
            //    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            //}
            //else
            //{
                GL.BufferData(BufferTarget.ElementArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.StaticDraw);
            //}

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void SubmitVerts(byte[] vertices)
        {
            if (VBO == 0) return;
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            if (vertices != null && vertices.Length > 0)
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length, vertices, BufferUsageHint.StaticDraw);
            else
                GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void SubmitInds(byte[] indices)
        {
            if (EBO == 0) return;
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            if (indices != null && indices.Length > 0)
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length, indices, BufferUsageHint.StaticDraw);
            else
                GL.BufferData(BufferTarget.ElementArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
        }

        //public void UpdateTriangles(uint[] _indices)
        //{
        //    SubmitInds(_indices);
        //}

        public void Bind()
        {
            GL.BindVertexArray(VAO);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteVertexArrays(1, ref VAO);
        }

        ~SceneMesh()
        {
            Console.WriteLine("Warning: GC on undisposed Mesh");
            Dispose();
        }
    }
}
