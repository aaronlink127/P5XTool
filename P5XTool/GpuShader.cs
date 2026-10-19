using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace P5XTool
{
    internal class GpuShader : IDisposable
    {
        public int shaderId;
        public GpuShader(string shaderSource, ShaderType shaderType)
        {
            shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, shaderSource);
            GL.CompileShader(shaderId);
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int vStatus);
            if (vStatus != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shaderId);
                throw new Exception("Vertex shader compilation failed: " + infoLog);
            }
        }
        public void Dispose()
        {
            GL.DeleteShader(shaderId);
            GC.SuppressFinalize(this);
        }
        ~GpuShader()
        {
            Console.WriteLine("Warning: GC on undisposed Shader");
            Dispose();
        }
    }
}
