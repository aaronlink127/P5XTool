using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace P5XTool
{
    public unsafe class SceneGlobals : IDisposable
    {
        public int UBO;
        public int SSBO;

        private const int MaxBones = 512;
        private const int BoneMatrixSize = 64;
        private const int SSBOSize = MaxBones * BoneMatrixSize;

        private const int mModel_Offset = 0;
        private const int mView_Offset = mModel_Offset + 64;
        private const int mProjection_Offset = mView_Offset + 64;
        private const int mNumBones_Offset = mProjection_Offset + 64;
        private const int mSunDirection_Offset = (mNumBones_Offset + 4 + 15) & ~15;

        private const int uboSize = mSunDirection_Offset + 16;

        public SceneGlobals()
        {
            GL.GenBuffers(1, out UBO);
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferData(BufferTarget.UniformBuffer, uboSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            GL.GenBuffers(1, out SSBO);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSBO);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, SSBOSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        public void UpdateModel(Matrix4 model)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, mModel_Offset, 64, ref model);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void UpdateView(Matrix4 view)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, mView_Offset, 64, ref view);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void UpdateProjection(Matrix4 projection)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, mProjection_Offset, 64, ref projection);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void UpdateMVP(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, mModel_Offset, 64, ref model);
            GL.BufferSubData(BufferTarget.UniformBuffer, mView_Offset, 64, ref view);
            GL.BufferSubData(BufferTarget.UniformBuffer, mProjection_Offset, 64, ref projection);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void UpdateBones(Matrix4[] bones)
        {
            int numBones = bones.Length;
            if (numBones > MaxBones)
                throw new Exception("Too many bones for SSBO");

            // update bone count in UBO
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, mNumBones_Offset, sizeof(int), ref numBones);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            // update SSBO bone matrices
            fixed (Matrix4* ptr = bones)
            {
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSBO);
                GL.BufferSubData(BufferTarget.ShaderStorageBuffer, 0, numBones * BoneMatrixSize, (IntPtr)ptr);
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
            }
        }

        public void UpdateSunDirection(Vector3 sunDir)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, UBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, mSunDirection_Offset, 12, ref sunDir);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void Bind()
        {
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, UBO);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, SSBO);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GL.DeleteBuffer(UBO);
            GL.DeleteBuffer(SSBO);
        }
        ~SceneGlobals()
        {
            Dispose();
        }
    }
}