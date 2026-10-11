using MiniEngine.Core;
using Silk.NET.OpenGL;

namespace MiniEngine.OpenGL.Textures;

public class Texture : IDisposable
{
    private readonly GL _gl;
    private readonly Func<bool> _isContextCurrent;
    private readonly int _ownerThread = System.Environment.CurrentManagedThreadId;
    private bool _disposed;

    public uint Handle { get; }

    public int Width { get; }
    public int Height { get; }

    public Texture(GL gl, ImageData img, Func<bool> isContextCurrent)
    {
        ArgumentNullException.ThrowIfNull(img);
        _gl = gl;
        _isContextCurrent = isContextCurrent;
        EnsureThread();

        Width = img.Width;
        Height = img.Height;

        Handle = _gl.GenTexture();

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

            }
        }
    }

    public void Bind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        EnsureThread();
        _gl.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        EnsureThread();
        _gl.DeleteTexture(Handle);
        _disposed = true;
    }

    private void EnsureThread()
    {
        if (System.Environment.CurrentManagedThreadId != _ownerThread || !_isContextCurrent())
            throw new InvalidOperationException("OpenGL textures require their owning thread and a current graphics context.");
    }

}
