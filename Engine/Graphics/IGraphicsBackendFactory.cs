namespace MiniEngine.Graphics;

public interface IGraphicsBackendFactory
{
    IGraphicsBackend Create(GraphicsContext context);
}