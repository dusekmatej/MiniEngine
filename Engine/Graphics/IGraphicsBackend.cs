using MiniEngine.Core;

namespace MiniEngine.Graphics;

public interface IGraphicsBackend
{
    void Clear();
    BackendTextureHandle CreateTexture(ImageData image);
    void DrawTexture(BackendTextureHandle texture, float x, float y, float width, float height);
}