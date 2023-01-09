using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Renderer : Component
    {
        public Model model { get; private set;}
        public string shader { get; private set; }
        public List<Texture> textures { get; private set; } = new List<Texture>();

        private uint _vertexBufferObject;
        private uint _colorBufferObject;
        private uint _vertexArrayObject;
        private uint _elementBufferObject;

        public Renderer(Model model, string shader = "default", string[]? textures = null)
        {
            this.model = model;
            this.shader = shader;

            foreach(string s in textures)
            {
                this.textures.Add(ShaderManager.CreateTexture(s));
            }
        }

        public Renderer(Renderer renderer)
        {
            model = renderer.model;
            shader = renderer.shader;
            textures = renderer.textures;
        }

        public override unsafe void Initialize()
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
            Shader program = ShaderManager.LoadProgram(shader);
            ShaderManager.ActivateShaderProgram(shader);
            // --- ------ --- //

            // --- TEXTURE --- //
            for (uint i = 0; i < textures.Count; i++)
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
        }

        public override unsafe void Update()
        {
            for (int i = 0; i < textures.Count; i++)
            {
                textures[i].Use((TextureUnit)(TextureUnit.Texture0 + i));
            }

            Shader program = ShaderManager.ActivateShaderProgram(shader); //

            var _rotation = (Matrix4X4.CreateRotationX<float>(((float)Math.PI / 180) * entity.Rotation.X) * Matrix4X4.CreateRotationY<float>(((float)Math.PI / 180) * entity.Rotation.Y) * Matrix4X4.CreateRotationZ<float>(((float)Math.PI / 180) * entity.Rotation.Z));
            var _model = Matrix4X4<float>.Identity * Matrix4X4.CreateScale<float>(entity.scale) * Matrix4X4.CreateTranslation<float>(entity.Position) * _rotation;

            program.SetFloat("fade", Program.loop * Program.loop);

            program.SetMatrix4("model", _model);
            program.SetMatrix4("view", Program._view);
            program.SetMatrix4("projection", Program._projection);

            Program.GL.BindVertexArray(_vertexArrayObject);
            Program.GL.DrawElements(GLEnum.Triangles, (uint)model.indices.Length, GLEnum.UnsignedInt, null);
        }
    }
}
