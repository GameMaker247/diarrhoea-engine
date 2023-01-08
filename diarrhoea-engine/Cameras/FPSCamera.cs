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

        /*
        // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private Vector3D<float> _front = -Vector3D<float>.UnitZ;

        private Vector3D<float> _up = Vector3D<float>.UnitY;

        private Vector3D<float> _right = Vector3D<float>.UnitX;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        public FPSCamera(Vector3D<float> position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        // The position of the camera
        public Vector3D<float> Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the projection matrix.
        public float AspectRatio { private get; set; }

        public Vector3D<float> Front { get { return _front; } set { _front = value; } }

        public Vector3D<float> Up { get { return _up; } set { _up = value; } }

        public Vector3D<float> Right { get { return _right; } set { _right = value; } }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Pitch
        {
            get => MathHelper.Rad2Deg(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.Deg2Rad(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Yaw
        {
            get => MathHelper.Rad2Deg(_yaw);
            set
            {
                _yaw = MathHelper.Deg2Rad(value);
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        // This has been discussed more in depth in a previous tutorial,
        // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float FOV
        {
            get => MathHelper.Rad2Deg(_fov);
            private set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.Deg2Rad(angle);
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public override Matrix4X4<float> GetViewMatrix()
        {
            return Matrix4X4.CreateLookAt<float>(Position, Position + Front, Up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public override Matrix4X4<float> GetProjectionMatrix()
        {
            return Matrix4X4.CreatePerspectiveFieldOfView<float>(FOV, AspectRatio, 0.01f, 100f);
        }

        public override void Zoom(float zoom)
        {
            FOV += zoom;
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            _front = Vector3D.Normalize(_front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3D.Normalize(Vector3D.Cross(_front, Vector3D<float>.UnitY));
            _up = Vector3D.Normalize(Vector3D.Cross(_right, _front));
        }
        */
    }
}
