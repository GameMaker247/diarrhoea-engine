using DiarrhoeaEngine;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Entity
    {
        public string name { get; private set; }
        public Model model { get; private set; }
        public string shader;
        
        private string textureSRC;
        private uint texture;
 
        public Vector3D<float> position { get; private set; } = Vector3D<float>.Zero;
        public Quaternion<float> rotation { get; private set; } = Quaternion<float>.Identity;
        
        public Entity(string name, Model model, string texture, string shader="default")
        {
            this.name = name;
            this.model = model;
            this.shader = shader;
            this.textureSRC = texture;
           
            Setup();
            //LoadTexture();     
        }

        public void MoveXYZ(float x, float y, float z)
        {
            position += new Vector3D<float>(x, y, z);
        }

        private uint _vertexBufferObject;
        private uint _vertexArrayObject;
        private uint _elementBufferObject;

        private unsafe void Setup()
        {
            _vertexBufferObject = Program.GL.GenBuffer();
            Program.GL.BindBuffer(GLEnum.ArrayBuffer, _vertexBufferObject);
            Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.vertices.AsSpan(), GLEnum.StaticDraw);

            _vertexArrayObject = Program.GL.GenVertexArray();
            Program.GL.BindVertexArray(_vertexArrayObject);
            Program.GL.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), null);
            Program.GL.EnableVertexAttribArray(0);

            _elementBufferObject = Program.GL.GenBuffer();
            Program.GL.BindBuffer(GLEnum.ElementArrayBuffer, _elementBufferObject);
            Program.GL.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)model.indices.AsSpan(), GLEnum.StaticDraw);

            Program.shader.LoadProgram(shader);
            Program.shader.ActivateShaderProgram(shader);
        }

        public unsafe void Draw()
        {
            Program.shader.ActivateShaderProgram(shader);
            Program.GL.BindVertexArray(_vertexArrayObject);
            Program.GL.DrawElements(GLEnum.Triangles, (uint)model.indices.Length, GLEnum.UnsignedInt, null);
        }

        private unsafe void LoadTexture()
        {
            Bitmap bmp = new Bitmap(textureSRC);
            bmp.RotateFlip(RotateFlipType.Rotate270FlipY);

            uint _texture = Program.GL.GenTexture();
            Program.GL.BindTexture(TextureTarget.Texture2D, _texture);

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Program.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)Silk.NET.OpenGL.PixelFormat.Rgba, (uint)bmp.Width, (uint)bmp.Height, 0, GLEnum.Bgra, GLEnum.UnsignedByte, (void*)bmpData.Scan0);

            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            Program.GL.GenerateTextureMipmap(_texture);
        }
    }
}

/*
 *             uint vao = Program.GL.GenVertexArray();
            Program.GL.BindVertexArray(vao);

            uint vertices = Program.GL.GenBuffer();
            uint indices = Program.GL.GenBuffer();
            uint texture = Program.GL.GenBuffer();

            float[] _verticesArray = new float[]
            {
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f
            };
            uint[] indicesArray = new uint[]
            {
                0, 1, 2,
                0, 3, 2
            };
            /*
            float[] colorArray = new float[]
            {   //1.0f, 0.0f, 0.0f, 1.0f,
                //0.0f, 0.0f, 1.0f, 1.0f,
                //0.0f, 1.0f, 0.0f, 1.0f,
                //0.5f, 0.5f, 0.5f, 1.0f
                0.0f, 0.0f, //lower-left corner
                0.0f, 1.0f, //top-left corner
                1.0f, 1.0f, //top-right corner
                1.0f, 0.0f //lower-right corner
            };*/
/*
Program.shader.ActivateShaderProgram(shader);
uint program = Program.shader.active;
Program.GL.UseProgram(program);

uint vPos = (uint)Program.GL.GetAttribLocation(program, "aPosition");
Program.GL.EnableVertexAttribArray(vPos);
Program.GL.VertexAttribPointer(vPos, 3, GLEnum.Float, false, 0, null);

Program.GL.BindBuffer(GLEnum.ArrayBuffer, vertices);
Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.vertices.AsSpan(), GLEnum.StaticDraw);
*/

/*
uint vTex = (uint)Program.GL.GetAttribLocation(program, "vTex");
Program.GL.BindBuffer(GLEnum.ArrayBuffer, texture);
Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.texture.AsSpan(), GLEnum.StaticDraw);
Program.GL.EnableVertexAttribArray(vTex);
Program.GL.VertexAttribPointer(vTex, 2, GLEnum.Float, false, 0, null);
*/
/*
Program.GL.BindBuffer(GLEnum.ElementArrayBuffer, indices);
Program.GL.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)model.indices.AsSpan(), GLEnum.StaticDraw);

Program.GL.BindBuffer(GLEnum.ArrayBuffer, 0);
Program.GL.DrawElements(GLEnum.Triangles, 6, GLEnum.UnsignedInt, null);

Program.GL.BindBuffer(GLEnum.ElementArrayBuffer, 0);
Program.GL.BindVertexArray(vao);

Program.GL.DeleteBuffer(vertices);
Program.GL.DeleteBuffer(indices);
Program.GL.DeleteBuffer(texture);
Program.GL.DeleteVertexArray(vao);
*//*
float[] vertexArray = new float[] {
    -0.5f, 0.5f, 0.0f,
    0.5f, 0.5f, 0.0f,
    0.5f, -0.5f, 0.0f,
    -0.5f, -0.5f, 0.0f
};

float[] colorArray = new float[] {
    1.0f, 0.0f, 0.0f, 1.0f,
    0.0f, 0.0f, 1.0f, 1.0f,
    0.0f, 1.0f, 0.0f, 1.0f,
    0.5f, 0.5f, 0.5f, 1.0f
};

uint[] indexArray = new uint[] 
{ 
    0, 1, 2,
    0, 3, 2
};
*/



/*
uint vao = Program.GL.GenVertexArray();
Program.GL.BindVertexArray(vao);

uint vbo = Program.GL.GenBuffer();
uint ebo = Program.GL.GenBuffer();
tbo = Program.GL.GenBuffer();

uint program = Program.shader.GetShaderProgram(shader);


Program.GL.BindBuffer(GLEnum.ArrayBuffer, vbo);
Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.vertices.AsSpan(), GLEnum.StaticDraw);
Program.GL.EnableVertexAttribArray(0);
Program.GL.VertexAttribPointer(0, 3, GLEnum.Float, false, 0, null);

Program.GL.Enable(EnableCap.Blend);
Program.GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

uint vertexLocation = (uint)Program.GL.GetAttribLocation(program, "vPos");
Program.GL.EnableVertexAttribArray(vertexLocation);
Program.GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);

uint texCoordLocation = (uint)Program.GL.GetAttribLocation(program, "vTex");

Program.GL.BindBuffer(GLEnum.ArrayBuffer, tbo);
Program.GL.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)model.texture.AsSpan(), GLEnum.StaticDraw);
Program.GL.EnableVertexAttribArray(texCoordLocation);
Program.GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), null);

Program.GL.BindBuffer(GLEnum.ElementArrayBuffer, ebo);
Program.GL.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)model.indices.AsSpan(), GLEnum.StaticDraw);
Program.GL.BindBuffer(GLEnum.ArrayBuffer, 0);
Program.GL.DrawElements(GLEnum.TriangleStrip, 6, GLEnum.UnsignedInt, null);

Program.GL.BindBuffer(GLEnum.ElementArrayBuffer, 0);
Program.GL.BindVertexArray(vao);

Program.GL.DeleteBuffer(vbo);
Program.GL.DeleteBuffer(ebo);
Program.GL.DeleteVertexArray(vao);*/