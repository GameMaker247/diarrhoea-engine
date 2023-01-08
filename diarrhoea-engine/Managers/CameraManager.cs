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

        public CameraManager(Camera initial)
        {
            active = initial;
            cameras.Add(active);
        }

        public void Enable(Type type)
        {
            Camera enable = cameras.Find(x => x.GetType() == type);
            if (enable == null) return;

            active = enable;
        }

        public void Create(Camera camera) 
        { 
            cameras.Add(camera);
        }

        public void Update()
        {

        }

        public void Render()
        {

        }
    }
}
