using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class FPSCamera : Camera
    {
        private float _FOV, _AspectRatio;

        public FPSCamera(float _FOV, float _AspectRatio, Vector3D<float> Position) : base(Position)
        {
            this._FOV = _FOV;
            this._AspectRatio = _AspectRatio;
        }

        public override Matrix4X4<float> GetProjectionMatrix()
        {
            return Matrix4X4.CreatePerspectiveFieldOfView<float>(MathHelper.Deg2Rad(_FOV), _AspectRatio, 0.01f, 100f);
        }

        public override Matrix4X4<float> GetViewMatrix()
        {
            return Matrix4X4.CreateLookAt<float>(Position, Position + Forward, Up);
        }

        public override void Zoom(float zoom)
        {
            _FOV = MathHelper.Clamp(_FOV+zoom, 10.0f, 120.0f);
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
