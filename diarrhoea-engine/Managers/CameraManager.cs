using Silk.NET.Maths;
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
        private Camera active;
        private List<Camera> cameras = new List<Camera>();

        public Vector3D<float> position = new Vector3D<float>(0.0f, -1.0f, -3.0f);
        public float rotation = 0.0f;
        public float yaw = 0.0f;
        public float FOV = 90.0f;

        public unsafe void Render()
        {
            //int location = (int)Program.GL.GetUniformLocation(Program.shader.active, "projection");

            //Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI/180)*60.0f, (float)Program.GetWindowSize().X / (float)Program.GetWindowSize().Y, 0.0001f, 5000.0f);

            //Program.GL.UniformMatrix4(location, 1, false, (double*)&projection);
        }
    }
}
