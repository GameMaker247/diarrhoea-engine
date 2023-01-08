using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class MathHelper
    {
        public static float PiOver2 = 1.570796f;
        public static float Deg2Rad(float x)
        {
            return (float)Math.PI / 180 * x;
        }
        public static float Rad2Deg(float x)
        {
            return x * (180.0f / (float)Math.PI);
        }
        public static float Clamp(float value, float min, float max) 
        {
            if (value <= min) return min;
            else if (value >= max) return max;
            else return value;
        }
        public static float Distance(Vector3D<float> a, Vector3D<float> b) 
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
        }
    }
}
