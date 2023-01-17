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

        public static Model LoadNewModel(string src)
        {
            if(models.TryGetValue(src, out Model model)) return model;

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            Assimp.Scene scene = importer.ImportFile(src);

            List<float> vertices = new List<float>();
            for(int i = 0; i < scene.Meshes[0].Vertices.Count; i++)
            {
                vertices.Add(scene.Meshes[0].Vertices[i].X);
                vertices.Add(scene.Meshes[0].Vertices[i].Y);
                vertices.Add(scene.Meshes[0].Vertices[i].Z);
            }

            List<uint> indices = new List<uint>();
            for(int i = 0; i < scene.Meshes[0].GetIndices().Length; i++)
            {
                indices.Add((uint)scene.Meshes[0].GetIndices()[i]);
            }

            List<float> textures = new List<float>();
            for (int i = 0; i < scene.Meshes[0].TextureCoordinateChannels[0].Count; i++)
            {
                textures.Add(scene.Meshes[0].TextureCoordinateChannels[0][i].X);
                textures.Add(1-scene.Meshes[0].TextureCoordinateChannels[0][i].Y);
            }

            Model result = new Model(vertices.ToArray(), indices.ToArray(), textures.ToArray());
            models.Add(src, result);
            return result;
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
