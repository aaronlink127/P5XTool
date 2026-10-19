using AssetRipper.Conversions.UnityCrunch;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using P5XLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAssetLib;
using static System.Runtime.InteropServices.JavaScript.JSType;
using GLWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;
using UnityWrapMode = UnityAssetLib.TextureWrapMode;
namespace P5XTool
{
    public static class GpuUtils
    {
        public static InternalFormat GetFormatForTexture(TextureFormat fmt, TextureColorSpace colorSpace)
        {
            if (colorSpace == TextureColorSpace.sRGB)
                return fmt switch
                {
                    TextureFormat.DXT1 => InternalFormat.CompressedSrgbAlphaS3tcDxt1Ext,
                    TextureFormat.DXT5 => InternalFormat.CompressedSrgbAlphaS3tcDxt5Ext,
                    TextureFormat.BC7 => InternalFormat.CompressedSrgbAlphaBptcUnorm,
                    TextureFormat.RGB24 => InternalFormat.Srgb8,
                    TextureFormat.RGBA32 => InternalFormat.Srgb8Alpha8,
                    _ => throw new Exception($"Unsupported sRGB format {fmt}")
                };
            return fmt switch
            {
                TextureFormat.DXT1 => InternalFormat.CompressedRgbaS3tcDxt1Ext,
                TextureFormat.DXT5 => InternalFormat.CompressedRgbaS3tcDxt5Ext,
                TextureFormat.BC7 => InternalFormat.CompressedRgbaBptcUnorm,
                TextureFormat.RGB24 => InternalFormat.Rgb8,
                TextureFormat.RGBA32 => InternalFormat.Rgba8,
                _ => throw new Exception($"Unsupported linear format {fmt}")
            };
        }
        private static GLWrapMode UnityToGLWrap(UnityWrapMode wrapMode)
        {
            return wrapMode switch
            {
                UnityWrapMode.Repeat => GLWrapMode.Repeat,
                UnityWrapMode.Clamp => GLWrapMode.ClampToEdge,
                UnityWrapMode.Mirror => GLWrapMode.MirroredRepeat,
                UnityWrapMode.MirrorOnce => GLWrapMode.ClampToEdge,
                _ => GLWrapMode.Repeat
            };
        }
        public static GpuTexture LoadUnityTexture(Texture textureAsset)
        {
            if (textureAsset is Texture2D texture2D)
            {
                var imageData = TexUtils.DeCrunch(texture2D.GetImageData(), texture2D.m_TextureFormat);
                var texFmt = TexUtils.GetUncrunched(texture2D.m_TextureFormat);
                InternalFormat internalFormat = GetFormatForTexture(texFmt, texture2D.m_ColorSpace);

                var sceneTex = new GpuTexture(texture2D.m_Width, texture2D.m_Height, internalFormat, imageData, texture2D.m_MipCount, texture2D.m_TextureSettings.m_MipBias, UnityToGLWrap(texture2D.m_TextureSettings.m_WrapU), UnityToGLWrap(texture2D.m_TextureSettings.m_WrapV));
                return sceneTex;
            }
            throw new Exception($"Unsupported texture type {textureAsset.GetType().Name}");
        }
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
            BlendIndices = 13
        }
        private static int GetFormatSize(ChannelInfo channel)
        {
            switch (channel.format)
            {
                case VertexFormat.Float:
                case VertexFormat.UInt32:
                case VertexFormat.SInt32:
                    return 4;
                case VertexFormat.Float16:
                case VertexFormat.UNorm16:
                case VertexFormat.SNorm16:
                case VertexFormat.UInt16:
                case VertexFormat.SInt16:
                    return 2;
                case VertexFormat.UNorm8:
                case VertexFormat.SNorm8:
                case VertexFormat.UInt8:
                case VertexFormat.SInt8:
                    return 1;
            }
            return 0;
        }
        public static SceneMesh LoadUnityMesh(Mesh meshAsset)
        {
            Console.WriteLine($"Loading mesh {meshAsset.m_Name}");
            var vertexData = meshAsset.GetVertexData().ToArray();
            List<uint> vertexSizes = [];
            var channels = meshAsset.m_VertexData.m_Channels;
            foreach (var channel in channels)
            {
                while (channel.stream >= vertexSizes.Count)
                {
                    vertexSizes.Add(0);
                }
                uint fmtSize = (uint)GetFormatSize(channel);
                uint afterElement = channel.offset + fmtSize * (uint)(channel.dimension & 0xF);
                if (afterElement > vertexSizes[channel.stream])
                {
                    vertexSizes[channel.stream] = afterElement;
                }
            }
            var streamOffsets = new uint[vertexSizes.Count];
            uint curOffset = 0;
            for (int i = 0; i < streamOffsets.Length; i++)
            {
                streamOffsets[i] = curOffset;
                curOffset += vertexSizes[i] * meshAsset.m_VertexData.m_VertexCount;
                curOffset = (uint)Math.Ceiling(curOffset / 16.0) * 16;
            }
            var indices = meshAsset.m_IndexBuffer;
            int getChannelOffset(int channelIdx)
            {
                var channel = channels[channelIdx];
                return (int)(streamOffsets[channel.stream] + channel.offset);
            }
            int getChannelStride(int channelIdx)
            {
                var channel = channels[channelIdx];
                return (int)(vertexSizes[channel.stream]);
            }
            VertexAttribPointerType getChannelFormat(int channelIdx)
            {
                var channel = channels[channelIdx];
                return channel.format switch
                {
                    VertexFormat.Float => VertexAttribPointerType.Float,
                    VertexFormat.Float16 => VertexAttribPointerType.HalfFloat,
                    VertexFormat.UInt8 => VertexAttribPointerType.UnsignedByte,
                    VertexFormat.UInt16 => VertexAttribPointerType.UnsignedShort,
                    VertexFormat.UInt32 => VertexAttribPointerType.UnsignedInt,
                    _ => VertexAttribPointerType.Float,
                };
            }
            SceneMesh.VertexAttributeDesc getChannelAttributeDesc(int channelIdx)
            {
                return new SceneMesh.VertexAttributeDesc()
                {
                    offset = getChannelOffset(channelIdx),
                    format = getChannelFormat(channelIdx),
                    stride = getChannelStride(channelIdx),
                    dimension = channels[channelIdx].dimension & 0xF
                };
            }
            var mesh = new SceneMesh();
            //mesh.positionAttr = getChannelAttributeDesc((int)VertexAttribute.Position);
            //mesh.normalAttr = getChannelAttributeDesc((int)VertexAttribute.Normal);
            //mesh.tangentAttr = getChannelAttributeDesc((int)VertexAttribute.Tangent);
            //mesh.uvAttr = getChannelAttributeDesc((int)VertexAttribute.TexCoord0);
            //mesh.boneIndsAttr = getChannelAttributeDesc((int)VertexAttribute.BlendIndices);
            //mesh.boneWeightsAttr = getChannelAttributeDesc((int)VertexAttribute.BlendWeight);
            for (int i = 0; i < channels.Length; i++)
            {
                var attrDesc = getChannelAttributeDesc(i);
                mesh.vertexAttributes[i] = attrDesc;
            }
            mesh.elementsType = meshAsset.m_IndexFormat == IndexFormat.UInt16 ? DrawElementsType.UnsignedShort : DrawElementsType.UnsignedInt;
            foreach (var submesh in meshAsset.m_SubMeshes)
            {
                var scsubmesh = new SceneSubMesh()
                {
                    BeginMode = submesh.topology switch
                    {
                        GfxPrimitiveType.Triangles => BeginMode.Triangles,
                        GfxPrimitiveType.TriangleStrip => BeginMode.TriangleStrip,
                        GfxPrimitiveType.Quads => BeginMode.Quads,
                        GfxPrimitiveType.Lines => BeginMode.Lines,
                        GfxPrimitiveType.LineStrip => BeginMode.LineStrip,
                        _ => BeginMode.Triangles,
                    },
                    firstByte = submesh.firstByte,
                    indexCount = submesh.indexCount
                };
                mesh.subMeshes.Add(scsubmesh);
            }
            var bindPoses = new Matrix4[meshAsset.m_BindPose.Length];
            for (int i = 0; i < bindPoses.Length; i++)
            {
                var mtx = meshAsset.m_BindPose[i];
                bindPoses[i] = new(
                    mtx.M11, mtx.M12, mtx.M13, mtx.M14,
                    mtx.M21, mtx.M22, mtx.M23, mtx.M24,
                    mtx.M31, mtx.M32, mtx.M33, mtx.M34,
                    mtx.M41, mtx.M42, mtx.M43, mtx.M44
                );
            }
            mesh.bindPose = bindPoses;
            mesh.InitBuffers();
            mesh.SubmitVerts(vertexData);
            mesh.SubmitInds(indices);
            //Console.WriteLine($"Loaded BindPose with {bindPoses.Length} elements.");
            return mesh;
        }
    }
}
