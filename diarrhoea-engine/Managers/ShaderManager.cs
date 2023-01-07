using Silk.NET.OpenGL;

namespace DiarrhoeaEngine
{
    public class ShaderManager
    {
        private Dictionary<string, uint> shaders = new Dictionary<string, uint>();
        public uint active { get; private set; }

        public void ActivateShaderProgram(string title)
        {
            if (!shaders.ContainsKey(title)) throw new Exception($"Shader {title} has not been loaded using LoadProgram.");

            Program.GL.UseProgram(0);
            Program.GL.UseProgram(shaders[title]);
            active = shaders[title];
        }

        public bool HasLoadedProgram(string title)
        {
            return shaders.ContainsKey(title);
        }

        public void LoadProgram(string title="default")
        {
            if (shaders.ContainsKey(title)) return;

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

            shaders.Add(title, program);
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
