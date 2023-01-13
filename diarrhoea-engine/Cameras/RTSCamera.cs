using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class RTSCamera : Camera
    {
        private float _width, _height, _ratio;
        private const float SPEED = 6.0f;

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

        public RTSCamera(float _width, float _height, Vector3D<float> Position) : base(Position)
        {
            this._width = _width;
            this._height = _height;

            _ratio = _height / _width;
        }

        public override Matrix4X4<float> GetProjectionMatrix()
        {
            return Matrix4X4.CreateOrthographic<float>(_width, _height, 0.001f, 1000.0f);
        }

        public override Matrix4X4<float> GetViewMatrix()
        {
            return Matrix4X4.CreateLookAt<float>(Position, Position + Forward, Up);
        }

        public override void Movement(Vector2D<float> vectors)
        {
            Position += MathHelper.PlaneProjection(Up * SPEED / (float)Program.window.FramesPerSecond, Vector3D<float>.UnitY) +
                MathHelper.PlaneProjection(Vector3D.Normalize<float>(Vector3D.Cross<float>(Forward, Up)) * vectors.X * SPEED / (float)Program.window.FramesPerSecond, Vector3D<float>.UnitY);
        }

        public override void Zoom(float zoom)
        {
            _width += zoom;
            _height += zoom*_ratio;
        }

        public override void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            Forward.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            Forward.Y = MathF.Sin(_pitch);
            Forward.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            Forward = Vector3D.Normalize(Forward);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            Right = Vector3D.Normalize(Vector3D.Cross(Forward, Vector3D<float>.UnitY));
            Up = Vector3D.Normalize(Vector3D.Cross(Right, Forward));
        }
    }
}
