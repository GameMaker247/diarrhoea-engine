using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class CameraManager
    {
        private Matrix4x4 position; 
        private Matrix4x4 rotation;

        public bool enabled { get; private set; } = true;

        public unsafe void Render()
        {
            //int location = (int)Program.GL.GetUniformLocation(Program.shader.active, "projection");

            //Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI/180)*60.0f, (float)Program.GetWindowSize().X / (float)Program.GetWindowSize().Y, 0.0001f, 5000.0f);

            //Program.GL.UniformMatrix4(location, 1, false, (double*)&projection);
        }
    }
}
