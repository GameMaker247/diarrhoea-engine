using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace DiarrhoeaEngine
{
    public class Entity
    {
        public string name { get; private set; }
        public Model model { get; private set; }
        public string shader;

        private List<Texture> textures = new List<Texture>();

        public Vector3D<float> position { get; private set; }
        public Vector3D<float> rotation { get; private set; }
        public float scale { get; private set; }
        
        public Entity(string name, Model model, string shader="default", string[] textures = null, Vector3D<float>? position = null, Vector3D<float>? rotation = null, float scale = 0.0f)
        {
            this.name = name;
            this.model = model;
            this.shader = shader;

            if (position == null) this.position = Vector3D<float>.Zero;
            else this.position = (Vector3D<float>)position;

            if (rotation == null) this.rotation = Vector3D<float>.Zero;
            else this.rotation = (Vector3D<float>)rotation;
            this.scale = scale;

            foreach(string tex in textures)
            {
                this.textures.Add(Program.shader.CreateTexture(tex));
            }

            Setup();
        }

        public void MoveXYZ(float x, float y, float z)
        {
            position += new Vector3D<float>(x, y, z);
        }

        private uint _vertexBufferObject;
        private uint _colorBufferObject;
        private uint _vertexArrayObject;
        private uint _elementBufferObject;

        private unsafe void Setup()
        {
            // --- OBJECT --- //
            _vertexArrayObject = Program.GL.GenVertexArray();
            Program.GL.BindVertexArray(_vertexArrayObject);
            // --- ------ --- //

            // --- VERTICES --- //
            _vertexBufferObject = Program.GL.GenBuffer();
            Program.GL.BindBuffer(GLEnum.ArrayBuffer, _vertexBufferObject);
            Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.vertices.AsSpan(), GLEnum.StaticDraw);
            // --- -------- --- //

            // --- INDICES --- //
            _elementBufferObject = Program.GL.GenBuffer();
            Program.GL.BindBuffer(GLEnum.ElementArrayBuffer, _elementBufferObject);
            Program.GL.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)model.indices.AsSpan(), GLEnum.StaticDraw);
            // --- ------- --- //

            // --- SHADER --- //
            Shader program = Program.shader.LoadProgram(shader);
            Program.shader.ActivateShaderProgram(shader);
            // --- ------ --- //

            // --- TEXTURE --- //
            for(uint i = 0; i < textures.Count; i++) 
            {
                program.SetInt($"texture{i}", (int)i);
            }
            // --- ------- --- //

            // --- VERTICES --- //
            uint vertexLocation = (uint)Program.GL.GetAttribLocation(program.id, "aPosition");
            Program.GL.VertexAttribPointer(vertexLocation, 3, GLEnum.Float, false, 0, null);
            Program.GL.EnableVertexAttribArray(vertexLocation);
            // --- -------- --- //

            // --- TEXTURE --- //
            _colorBufferObject = Program.GL.GenBuffer();
            Program.GL.BindBuffer(GLEnum.ArrayBuffer, _colorBufferObject);
            Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.texture.AsSpan(), GLEnum.StaticDraw);

            uint textureLocation = (uint)Program.GL.GetAttribLocation(program.id, "aTexCoord");
            Program.GL.VertexAttribPointer(textureLocation, 2, GLEnum.Float, false, 0, null);
            Program.GL.EnableVertexAttribArray(textureLocation);
            // --- ------- --- //

            //Console.WriteLine($"Width: {Program.GetWindowSize().X}, Height: {Program.GetWindowSize().Y}, Ratio: {Program.GetWindowSize().X / Program.GetWindowSize().Y}");
        }

        public unsafe void Draw()
        {
            for (int i = 0; i < textures.Count; i++)
            {
                textures[i].Use((TextureUnit)(TextureUnit.Texture0+i));
            }

            Shader program = Program.shader.ActivateShaderProgram(shader); //

            var _rotation = Matrix4X4.CreateRotationX<float>(((float)Math.PI / 180) * rotation.X) * Matrix4X4.CreateRotationY<float>(((float)Math.PI / 180) * rotation.Y) * Matrix4X4.CreateRotationZ<float>(((float)Math.PI / 180) * rotation.Z);
            var _model = Matrix4X4<float>.Identity * Matrix4X4.CreateScale<float>(scale) * Matrix4X4.CreateTranslation<float>(position) * _rotation;

            program.SetFloat("fade", Program.loop* Program.loop);

            program.SetMatrix4("model", _model);
            program.SetMatrix4("view", Program._view);
            program.SetMatrix4("projection", Program._projection);

            Program.GL.BindVertexArray(_vertexArrayObject);
            Program.GL.DrawElements(GLEnum.Triangles, (uint)model.indices.Length, GLEnum.UnsignedInt, null);
        }
    }
}
