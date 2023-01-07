using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Model
    {
        public float[] vertices { get; private set; }
        public uint[] indices { get; private set; }
        public float[] texture { get; private set; }

        public Model(float[] floats, uint[] indices, float[] texture)
        {
            this.vertices = floats;
            this.indices = indices;
            this.texture = texture;
        }

        public static Model Square = new Model(new float[]
        {                
            0.5f, 0.5f, 0.0f,       // top-right
            0.5f, -0.5f, 0.0f,      // bottom-right
            -0.5f, -0.5f, 0.0f,     // bottom-left
            -0.5f, 0.5f, 0.0f       // top-left
        },
        new uint[]
        {
            0, 1, 3,
            1, 2, 3
        },
        new float[]
        {
            1.0f, 1.0f,            // top-right corner
            1.0f, 0.0f,            // bottom-right corner
            0.0f, 0.0f,            // bottom-left corner
            0.0f, 1.0f             // top-left corner
        });

        public static Model Cube = new Model(new float[]
        {
            0.5f, 0.5f, 0.5f,        // top-right-front
            0.5f, -0.5f, 0.5f,       // bottom-right-front
            -0.5f, -0.5f, 0.5f,      // bottom-left-front
            -0.5f, 0.5f, 0.5f,       // top-left-front
            0.5f, 0.5f, -0.5f,       // top-right-back
            0.5f, -0.5f, -0.5f,      // bottom-right-back
            -0.5f, -0.5f, -0.5f,     // bottom-left-back
            -0.5f, 0.5f, -0.5f       // top-left-back
        },
        new uint[]
        {
            //Top
            7, 4, 0, //need to check
            4, 2, 0, //this because i dk
            //Front
            0, 1, 3,
            1, 2, 3,

            //Back

        },
        new float[]
        {
            1.0f, 1.0f,            // top-right corner
            1.0f, 0.0f,            // bottom-right corner
            0.0f, 0.0f,            // bottom-left corner
            0.0f, 1.0f             // top-left corner
        });
    }
}
