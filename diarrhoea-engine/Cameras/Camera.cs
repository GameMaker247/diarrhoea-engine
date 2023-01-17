using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Camera
    {
        public Vector3D<float> Position = Vector3D<float>.Zero;
        public float _yaw;
        public float _pitch;

        public Camera(Vector3D<float> Position)
        {
            this.Position = Position;
        }

        public float Pitch
        {
            get => MathHelper.Rad2Deg(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.Deg2Rad(angle);
                UpdateVectors();
            }
        }
        public float Yaw
        {
            get => MathHelper.Rad2Deg(_yaw);
            set
            {
                _yaw = MathHelper.Deg2Rad(value);
                UpdateVectors();
            }
        }

        public Vector3D<float> Forward = -Vector3D<float>.UnitZ;
        public Vector3D<float> Right = Vector3D<float>.UnitX;
        public Vector3D<float> Up = Vector3D<float>.UnitY;

        public virtual void Zoom(float zoom) { }
        public virtual void UpdateVectors() { }
        public virtual void Movement(Vector2D<float> vectors) { throw new NotImplementedException(); }
        public virtual Matrix4X4<float> GetProjectionMatrix() { throw new NotImplementedException(); }
        public virtual Matrix4X4<float> GetViewMatrix() { throw new NotImplementedException(); }
    }
    /*
    public interface ICamera
    {
        public Matrix4X4<float> GetViewMatrix();
        public Matrix4X4<float> GetProjectionMatrix();
    }
    */
}
