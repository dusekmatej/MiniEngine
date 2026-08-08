using MiniEngine.Core;

namespace MiniEngine.Rendering;

public interface IGraphicsBackend
{
    void Clear();
    BackendTextureHandle CreateTexture(ImageData image);
    void DrawTexture(BackendTextureHandle texture, float x, float y, float width, float height);
}