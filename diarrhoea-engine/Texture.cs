using Silk.NET.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;

namespace DiarrhoeaEngine
{
    public class Texture
    {
        public uint id;
        public string src { get; private set; }

        public unsafe Texture(string src)
        {
            Bitmap bmp = new Bitmap(src);
            this.src = src;

            bmp.RotateFlip(RotateFlipType.Rotate90FlipY);               //.Format32bppArgb
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);//Format32bppArgb);

            id = Program.GL.GenTexture();
            Program.GL.BindTexture(TextureTarget.Texture2D, id);
            Program.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)Silk.NET.OpenGL.PixelFormat.Rgba, (uint)data.Width, (uint)data.Height, 0, GLEnum.Bgra, GLEnum.UnsignedByte, (void*)data.Scan0);

            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            Program.GL.GenerateTextureMipmap(id);
            bmp.Dispose();
        }

        public unsafe void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            Program.GL.ActiveTexture(unit);
            Program.GL.BindTexture(GLEnum.Texture2D, id);
        }
    }
}
