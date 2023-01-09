using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Model
    {
        public float[] vertices { get; private set; }
        public uint[] indices { get; private set; }
        public float[] texture { get; private set; }

        private static Dictionary<string, Model> models = new Dictionary<string, Model>();

        public Model(float[] floats, uint[] indices, float[] texture)
        {
            this.vertices = floats;
            this.indices = indices;
            this.texture = texture;
        }

        public static Model LoadModel(string src)
        {
            if (models.ContainsKey(src)) { models.TryGetValue(src, out Model result); return result; }

            string file = File.ReadAllText(src);

            Regex v = new Regex("v (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+");
            Regex vn = new Regex("vn (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+");
            Regex vt = new Regex("vt (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+");
            Regex f = new Regex("f [0-9]+/[0-9]+/[0-9]+ [0-9]+/[0-9]+/[0-9]+ [0-9]+/[0-9]+/[0-9]+");

            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            List<float> ts = new List<float>();
            List<uint> fs = new List<uint>();

            List<uint> indices = new List<uint>();

            MatchCollection collectionV = v.Matches(file);
            MatchCollection collectionVN = vn.Matches(file);
            MatchCollection collectionVT = vt.Matches(file);
            MatchCollection collectionF = f.Matches(file);

            for(int i = 0; i < collectionV.Count; i++)
            {
                string values = collectionV[i].Value.Split('v')[1];
                
                float x = float.Parse(values.Split(' ')[1]);
                float y = float.Parse(values.Split(' ')[2]);
                float z = float.Parse(values.Split(' ')[3]);

                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(z);
            }

            for (int i = 0; i < collectionVN.Count; i++)
            {
                string values = collectionVN[i].Value.Split("vn")[1];

                float x = float.Parse(values.Split(' ')[1]);
                float y = float.Parse(values.Split(' ')[2]);
                float z = float.Parse(values.Split(' ')[3]);

                normals.Add(x);
                normals.Add(y);
                normals.Add(z);
            }

            for (int i = 0; i < collectionVT.Count; i++)
            {
                string values = collectionVT[i].Value.Split("vt")[1];

                float x = float.Parse(values.Split(' ')[1]);
                float y = float.Parse(values.Split(' ')[2]);

                ts.Add(x);
                ts.Add(y);
            }

            for(int i = 0; i < collectionF.Count; i++)
            {
                string values = collectionF[i].Value.Split('f')[1];

                //Vertex Index
                uint vi_x = uint.Parse(values.Split(' ')[1].Split('/')[0]);
                uint vi_y = uint.Parse(values.Split(' ')[1].Split('/')[1]);
                uint vi_z = uint.Parse(values.Split(' ')[1].Split('/')[2]);

                //UV Index
                uint uv_x = uint.Parse(values.Split(' ')[2].Split('/')[0]);
                uint uv_y = uint.Parse(values.Split(' ')[2].Split('/')[1]);
                uint uv_z = uint.Parse(values.Split(' ')[2].Split('/')[2]);

                //Normal Index
                uint ni_x = uint.Parse(values.Split(' ')[3].Split('/')[0]);
                uint ni_y = uint.Parse(values.Split(' ')[3].Split('/')[1]);
                uint ni_z = uint.Parse(values.Split(' ')[3].Split('/')[2]);

                indices.Add(vi_x);
                indices.Add(vi_y);
                indices.Add(vi_z);
            }

            return new Model(vertices.ToArray(), indices.ToArray(), normals.ToArray());
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

            //Bottom
            6, 5, 2,
            5, 
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
