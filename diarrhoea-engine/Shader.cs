using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Shader
    {
        public uint id { get; private set; }
        public string name { get; private set; }

        public Shader(uint id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public void SetInt(string name, int value)
        {
            int location = Program.GL.GetUniformLocation(id, name);
            Program.GL.Uniform1(location, value);
        }

        public unsafe void SetMatrix4(string name, Matrix4X4<float> value)
        {
            int location = Program.GL.GetUniformLocation(id, name);
            Program.GL.UniformMatrix4(location, 1, false, (float*)&value);
        }
    }
}
