using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public Model(float[] vertices, uint[] indices, float[] texture)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.texture = texture;
        }

        public static Model LoadModel(string src)
        {
            //if (models.ContainsKey(src)) { models.TryGetValue(src, out Model result); return result; }

            string file = File.ReadAllText(src);

            Regex v = new Regex("v (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+");
            Regex vn = new Regex("vn (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+");
            Regex vt = new Regex("vt (-|)[0-9]+.[0-9]+ (-|)[0-9]+.[0-9]+");
            Regex f = new Regex("f [0-9]+/[0-9]+/[0-9]+ [0-9]+/[0-9]+/[0-9]+ [0-9]+/[0-9]+/[0-9]+");

            List<Vector3D<float>> vertices = new List<Vector3D<float>>();
            List<Vector2D<float>> textures = new List<Vector2D<float>>();
            List<Vector3D<float>> normals = new List<Vector3D<float>>();
            
            List<float> _vertices = new List<float>();
            List<float> _textures = new List<float>();
            List<float> _normals = new List<float>();
            List<uint> _indices = new List<uint>();

            MatchCollection regexVertices = v.Matches(file);
            MatchCollection regexNormals = vn.Matches(file);
            MatchCollection regexTextures = vt.Matches(file);
            MatchCollection regexFaces = f.Matches(file);

            for(int i = 0; i < regexVertices.Count; i++)
            {
                string values = regexVertices[i].Value.Split('v')[1];

                float x = float.Parse(values.Split(' ')[1]);
                float y = float.Parse(values.Split(' ')[2]);
                float z = float.Parse(values.Split(' ')[3]);

                vertices.Add(new Vector3D<float>(x, y, z));
            }

            for (int i = 0; i < regexTextures.Count; i++)
            {
                string values = regexTextures[i].Value.Split("vt")[1];

                float x = float.Parse(values.Split(' ')[1]);
                float y = float.Parse(values.Split(' ')[2]);

                textures.Add(new Vector2D<float>(x, y));
            }

            for (int i = 0; i < regexNormals.Count; i++)
            {
                string values = regexNormals[i].Value.Split("vn")[1];

                float x = float.Parse(values.Split(' ')[1]);
                float y = float.Parse(values.Split(' ')[2]);
                float z = float.Parse(values.Split(' ')[3]);

                normals.Add(new Vector3D<float>(x, y, z));
            }

            for(int i = 0; i < regexFaces.Count; i++)
            {
                string values = regexFaces[i].Value.Split('f')[1];

                //Vertex Index
                int vertex_1 = int.Parse(values.Split(' ')[1].Split('/')[0])-1;
                int texture_1 = int.Parse(values.Split(' ')[1].Split('/')[1])-1;
                int normal_1 = int.Parse(values.Split(' ')[1].Split('/')[2])-1;

                //UV Index
                int vertex_2 = int.Parse(values.Split(' ')[2].Split('/')[0])-1;
                int texture_2 = int.Parse(values.Split(' ')[2].Split('/')[1])-1;
                int normal_2 = int.Parse(values.Split(' ')[2].Split('/')[2])-1;

                //Normal Index
                int vertex_3 = int.Parse(values.Split(' ')[3].Split('/')[0])-1; // vertice Z
                int texture_3 = int.Parse(values.Split(' ')[3].Split('/')[1])-1; // texture Z
                int normal_3 = int.Parse(values.Split(' ')[3].Split('/')[2])-1; // normal Z
             
                _indices.Add((uint)vertex_1);
                _indices.Add((uint)vertex_2);
                _indices.Add((uint)vertex_3);

                _textures.Add(textures.ToArray()[texture_1].X);
                _textures.Add(textures.ToArray()[texture_1].Y);

                _textures.Add(textures.ToArray()[texture_2].X);
                _textures.Add(textures.ToArray()[texture_2].Y);

                _textures.Add(textures.ToArray()[texture_3].X);
                _textures.Add(textures.ToArray()[texture_3].Y);
            
                _normals.Add(normals[(int)normal_1].X);
                _normals.Add(normals[(int)normal_1].Y);
                _normals.Add(normals[(int)normal_1].Z);

                _normals.Add(normals[(int)normal_2].X);
                _normals.Add(normals[(int)normal_2].Y);
                _normals.Add(normals[(int)normal_2].Z);

                _normals.Add(normals[(int)normal_3].X);
                _normals.Add(normals[(int)normal_3].Y);
                _normals.Add(normals[(int)normal_3].Z);
            }

            for(int i = 0; i < vertices.Count; i++)
            {
                _vertices.Add(vertices[i].X);
                _vertices.Add(vertices[i].Y);
                _vertices.Add(vertices[i].Z);
            }

            return new Model(_vertices.ToArray(), _indices.ToArray(), _textures.ToArray());
        }

        private static void ProcessVertex()
        {

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
