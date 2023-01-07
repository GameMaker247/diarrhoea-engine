using Silk.NET.OpenGL;

namespace DiarrhoeaEngine
{
    public class ShaderManager
    {
        private List<Shader> shaders = new List<Shader>();
        private List<Texture> textures = new List<Texture>();

        private uint _active = 0;

        public Shader GetActive()
        {
            return shaders[(int)_active];
        }

        public Texture CreateTexture(string src)
        {
            if(textures.Find(x => x.src == src) != null) return textures.Find(x => x.src == src);

            Texture result = new Texture(src);
            textures?.Add(result);
            return result;
        }

        public Shader ActivateShaderProgram(string title)
        {
            Shader? shader = shaders.Find(x => x.name == title);
            if (shader == null) throw new Exception($"Shader {title} has not been loaded using LoadProgram.");

            Program.GL.UseProgram(0);
            Program.GL.UseProgram(shader.id);
            _active = (uint)shaders.IndexOf(shader);
            return shader;
        }

        public Shader LoadProgram(string title="default")
        {
            if (shaders.Exists(x => x.name == title)) return shaders.Find(x => x.name == title);

            uint program = Program.GL.CreateProgram();

            uint vertex = LoadShader(title, ShaderType.VertexShader);
            uint fragment = LoadShader(title, ShaderType.FragmentShader);

            Program.GL.AttachShader(program, vertex);
            Program.GL.AttachShader(program, fragment);
            Program.GL.LinkProgram(program);
            Program.GL.DetachShader(program, vertex);
            Program.GL.DetachShader(program, fragment);

            Program.GL.DeleteShader(vertex);
            Program.GL.DeleteShader(fragment);

            string info = Program.GL.GetProgramInfoLog(program);
            if (!string.IsNullOrEmpty(info)) throw new Exception(info);

            Shader result = new Shader(program, title);
            shaders.Add(result);
            return result;
        }

        private uint LoadShader(string title, ShaderType type)
        {
            uint id = Program.GL.CreateShader(type);

            switch(type)
            {
                case ShaderType.VertexShader:
                    {
                        Program.GL.ShaderSource(id, File.ReadAllText($"../../../Shaders/{title}/vertex.glsl"));
                    }
                    break;
                case ShaderType.FragmentShader:
                    {
                        Program.GL.ShaderSource(id, File.ReadAllText($"../../../Shaders/{title}/fragment.glsl"));
                    }
                    break;
            }

            Program.GL.CompileShader(id);
            string info = Program.GL.GetShaderInfoLog(id);
            if (!string.IsNullOrEmpty(info)) throw new Exception(info);

            return id; 
        }
    }
}
