using MiniEngine.Graphics;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace MiniEngine.OpenGL.Core;

public class BackendFactory : IGraphicsBackendFactory
{
    public IGraphicsBackend Create(GraphicsContext context)
    {
        var nativeContext = new LamdaNativeContext(
            context.GetProcAddress
        );

        var gl = GL.GetApi(nativeContext);

        return new Renderer(gl);
    }
}