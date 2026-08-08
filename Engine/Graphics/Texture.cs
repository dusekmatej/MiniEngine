using MiniEngine.Core;
using Silk.NET.OpenGL;

namespace MiniEngine.Graphics;

public class Texture
{
    private readonly GL _gl;

    public uint Id { get; }

    public int Width { get; }
    public int Height { get; }

    public Texture(GL gl,ImageData  img)
    {
        _gl = gl;

        Width = img.Width;
        Height = img.Height;

        Id = _gl.GenTexture();

        Bind();

        unsafe
        {
            fixed (byte* pixels = img.Pixels)
            {
                _gl.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    InternalFormat.Rgba,
                    (uint)Width,
                    (uint)Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    pixels
                );

                _gl.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int)GLEnum.Nearest
                );

                _gl.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter,
                    (int)GLEnum.Nearest
                );

                _gl.GenerateMipmap(TextureTarget.Texture2D);
            }
        }
    }

    public void Bind()
    {
        _gl.BindTexture(TextureTarget.Texture2D, Id);
    }

}