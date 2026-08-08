namespace MiniEngine.Rendering;

public interface IGraphicsBackendFactory
{
    IGraphicsBackend Create(GraphicsContext context);
}