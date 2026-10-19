using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public enum VertexFormat : byte
    {
        Float,
        Float16,
        UNorm8,
        SNorm8,
        UNorm16,
        SNorm16,
        UInt8,
        SInt8,
        UInt16,
        SInt16,
        UInt32,
        SInt32
    }
    public partial class ChannelInfo : Object
    {
        public byte stream;
        public byte offset;
        public VertexFormat format;
        public byte dimension;
    }
    public partial class VertexData : Object {
        public uint m_VertexCount;
        public ChannelInfo[] m_Channels = [];
        public byte[] m_DataSize = [];
    }
    public partial class AABB : Object
    {
        public Vector3 m_Center;
        public Vector3 m_Extent;
    }

    public enum GfxPrimitiveType : int
    {
        Triangles = 0,
        TriangleStrip = 1,
        Quads = 2,
        Lines = 3,
        LineStrip = 4,
        Points = 5
    }

    public partial class SubMesh : Object
    {
        public uint firstByte;
        public uint indexCount;
        public GfxPrimitiveType topology;
        public uint baseVertex;
        public uint firstVertex;
        public uint vertexCount;
        public AABB localAABB = new();
    }
    public enum IndexFormat
    {
        UInt16 = 0,
        UInt32 = 1
    }
    public partial class Mesh : Object
    {
        public string m_Name = "";
        public IndexFormat m_IndexFormat;
        public byte[] m_IndexBuffer = [];
        public SubMesh[] m_SubMeshes = [];
        public VertexData m_VertexData = new();
        public Matrix4x4[] m_BindPose = [];
        public StreamingInfo m_StreamData = new();
        public ReadOnlySpan<byte> GetVertexData()
        {
            if (m_VertexData.m_DataSize.Length != 0)
            {
                return m_VertexData.m_DataSize;
            }
            return m_StreamData.GetData();
        }
        public uint[] GetIndices()
        {
            if (m_IndexFormat == IndexFormat.UInt16)
            {
                uint[] ret = new uint[m_IndexBuffer.Length / 2];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = BitConverter.ToUInt16(m_IndexBuffer, i * 2);
                }
                return ret;
            }
            else
            {
                uint[] ret = new uint[m_IndexBuffer.Length / 4];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = BitConverter.ToUInt32(m_IndexBuffer, i * 4);
                }
                return ret;
            }
        }
    }
}
