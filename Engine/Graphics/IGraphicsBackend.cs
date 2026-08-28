using MiniEngine.Core;

namespace MiniEngine.Graphics;

public interface IGraphicsBackend
{
    void Clear();
    BackendTextureHandle CreateTexture(ImageData image);
    void DrawTexture(TextureDrawCommand command);
    void DrawRectangle(RectangleDrawCommand command);
}