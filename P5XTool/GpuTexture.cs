using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace P5XTool
{
    public class GpuTexture : IDisposable
    {
        public int textureID;
        public TextureTarget target;
        public GpuTexture(int textureID, TextureTarget target)
        {
            this.textureID = textureID;
            this.target = target;
        }
        public GpuTexture(int width, int height, InternalFormat internalFormat, byte[] data, int mipCount, float mipBias, TextureWrapMode wrapS, TextureWrapMode wrapT)
        {
            target = TextureTarget.Texture2D;
            textureID = GL.GenTexture();
            GL.BindTexture(target, textureID);

            int offset = 0;
            int w = width;
            int h = height;

            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.TextureCompressedBlockSize, 1, out int blockSize);
            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.TextureCompressedBlockWidth, 1, out int blockW);
            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.TextureCompressedBlockHeight, 1, out int blockH);
            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.InternalformatRedSize, 1, out int redSize);
            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.InternalformatGreenSize, 1, out int greenSize);
            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.InternalformatBlueSize, 1, out int blueSize);
            GL.GetInternalformat(ImageTarget.Texture2D, (SizedInternalFormat)internalFormat, InternalFormatParameter.InternalformatAlphaSize, 1, out int alphaSize);

            // Hack for NVIDIA GPUs
            if (blockSize > 16)
            {
                blockSize /= 8;
            }

            for (int level = 0; level < mipCount; level++)
            {
                int levelSize;

                if (blockSize != 0)
                {
                    int blocksX = (w + blockW - 1) / blockW;
                    int blocksY = (h + blockH - 1) / blockH;
                    levelSize = blocksX * blocksY * blockSize;
                    //Console.WriteLine($"blocksX {blocksX} blocksY {blocksY} levelSize {levelSize}");
                    GL.CompressedTexImage2D(target, level, internalFormat, w, h, 0, levelSize, data.AsSpan(offset, levelSize).ToArray());
                }
                else
                {
                    levelSize = w * h * ((redSize + greenSize + blueSize + alphaSize) / 8);
                    GL.TexImage2D(target, level, (PixelInternalFormat)internalFormat, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data.AsSpan(offset, levelSize).ToArray());
                }

                offset += levelSize;

                w = Math.Max(1, w >> 1);
                h = Math.Max(1, h >> 1);
            }

            GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)wrapS);
            GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)wrapT);

            GL.TexParameter(target, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(target, TextureParameterName.TextureMaxLevel, mipCount - 1);
            GL.TexParameter(target, TextureParameterName.TextureLodBias, mipBias);

            GL.BindTexture(target, 0);
        }
        public void Bind()
        {
            GL.BindTexture(target, textureID);
        }
        public void BindToUnit(int idx, int unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + idx);
            Bind();
            GL.Uniform1(unit, idx);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (textureID != 0)
            {
                GL.DeleteTexture(textureID);
                textureID = 0;
            }
        }
        ~GpuTexture()
        {
            Console.WriteLine("Warning: GC on undisposed Texture");
            Dispose();
        }
    }
}
