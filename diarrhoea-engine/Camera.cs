using System.Numerics;

namespace DiarrhoeaEngine
{
    public class Camera
    {
        private Matrix4x4 position;
        private Matrix4x4 rotation;

        public bool enabled { get; private set; } = true;

    }
}
