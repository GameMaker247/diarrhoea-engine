using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace DiarrhoeaEngine
{
    public class Entity
    {
        public string name { get; private set; }
        public Model model { get; private set; }
        public string shader;

        private Texture texture;
        private Texture texture2;

        public Vector3D<float> position { get; private set; } = Vector3D<float>.Zero;
        public Quaternion<float> rotation { get; private set; } = Quaternion<float>.Identity;
        
        public Entity(string name, Model model, string texture, string shader="default")
        {
            this.name = name;
            this.model = model;
            this.shader = shader;

            this.texture = new Texture(texture);
            this.texture2 = new Texture("../../../Images/bean.png");

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
            program.SetInt("texture1", 0); 
            program.SetInt("texture2", 1);

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

            Console.WriteLine($"Width: {Program.GetWindowSize().X}, Height: {Program.GetWindowSize().Y}, Ratio: {Program.GetWindowSize().X / Program.GetWindowSize().Y}");
        }

        public unsafe void Draw()
        {
            texture.Use();
            texture2.Use(TextureUnit.Texture1);

            Shader program = Program.shader.ActivateShaderProgram(shader);

            var _model = Matrix4X4<float>.Identity * Matrix4X4.CreateRotationX<float>(((float)Math.PI / 180) * -55.0f);

            program.SetMatrix4("model", _model);
            program.SetMatrix4("view", Program._view);
            program.SetMatrix4("projection", Program._projection);

            Program.GL.BindVertexArray(_vertexArrayObject);
            Program.GL.DrawElements(GLEnum.Triangles, (uint)model.indices.Length, GLEnum.UnsignedInt, null);
        }
    }
}
