using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
