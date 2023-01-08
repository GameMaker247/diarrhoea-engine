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
        public Vector3D<float> direction = new Vector3D<float>(0.0f, 0.0f, 0.0f);
        public Vector3D<float> right = new Vector3D<float>(0.0f, 0.0f, 0.0f);
        public Vector3D<float> up = new Vector3D<float>( 0.0f, 0.0f, 0.0f );
        public Vector3D<float> front = new Vector3D<float>(0.0f, 0.0f, 0.0f);

        public float pitch = 0.0f;
        public float yaw = 0.0f;

        //public float rotation = 0.0f;
        //public float yaw = 0.0f;
        public float FOV = 90.0f;

        public void Update()
        {
            Vector3D<float> target = Vector3D<float>.Zero;
            direction = Vector3D.Normalize<float>(position - target);

            Vector3D<float> _up = Vector3D<float>.UnitY;
            right = Vector3D.Normalize<float>(Vector3D.Cross<float>(_up, direction));

            up = Vector3D.Cross<float>(direction, right);

            front.X = (float)Math.Cos(((float)Math.PI / 180) * pitch) * (float)Math.Cos(((float)Math.PI / 180) * yaw);
            front.Y = (float)Math.Sin(((float)Math.PI / 180) * pitch);
            front.Z = (float)Math.Cos(((float)Math.PI / 180) * pitch) * (float)Math.Sin(((float)Math.PI / 180) * yaw);

            front = Vector3D.Normalize<float>(front);
        }

        public unsafe void Render()
        {
            //int location = (int)Program.GL.GetUniformLocation(Program.shader.active, "projection");

            //Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI/180)*60.0f, (float)Program.GetWindowSize().X / (float)Program.GetWindowSize().Y, 0.0001f, 5000.0f);

            //Program.GL.UniformMatrix4(location, 1, false, (double*)&projection);
        }
    }
}
