using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace P5XTool
{
    public class GpuProgram : IDisposable
    {
        public int programID;
        public int bindTexID;
        public GpuProgram(int programID)
        {
            this.programID = programID;
        }
        public GpuProgram(string vertexSource, string fragmentSource)
        {
            using var vertexShader = new GpuShader(vertexSource, ShaderType.VertexShader);
            using var fragmentShader = new GpuShader(fragmentSource, ShaderType.FragmentShader);
            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShader.shaderId);
            GL.AttachShader(programID, fragmentShader.shaderId);
            GL.LinkProgram(programID);
            GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(programID);
                throw new Exception("Program linking failed: " + infoLog);
            }
        }
        public void Bind()
        {
            bindTexID = 0;
            GL.UseProgram(programID);
        }
        public void SetUniform(string name, int value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }
        public void SetUniform(string name, float value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }
        public void SetUniform(string name, float v1, float v2)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform2(location, v1, v2);
            }
        }
        public void SetUniform(string name, float v1, float v2, float v3)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform3(location, v1, v2, v3);
            }
        }
        public void SetUniform(string name, float v1, float v2, float v3, float v4)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform4(location, v1, v2, v3, v4);
            }
        }
        public void SetUniform(string name, OpenTK.Mathematics.Vector2 value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform2(location, value);
            }
        }
        public void SetUniform(string name, OpenTK.Mathematics.Vector4 value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform4(location, value);
            }
        }
        public void Dispose()
        {
            GL.DeleteProgram(programID);
            GC.SuppressFinalize(this);
        }

        ~GpuProgram()
        {
            Console.WriteLine("Warning: GC on undisposed GpuProgram");
            GL.DeleteProgram(programID);
        }
    }
}
