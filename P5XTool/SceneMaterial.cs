using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace P5XTool
{
    public class SceneMaterial(GpuProgram shader)
    {
        public GpuProgram shader = shader;
        public Dictionary<string, GpuTexture> textures = [];

        public void Bind()
        {
            shader.Bind();
            int idx = shader.bindTexID;
            foreach (var kvp in textures)
            {
                int uniformLocation = GL.GetUniformLocation(shader.programID, kvp.Key);
                if (uniformLocation != -1)
                {
                    kvp.Value.BindToUnit(idx++, uniformLocation);
                }
            }
        }
    }
}
