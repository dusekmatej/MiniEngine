using MiniEngine.Core;

namespace MiniEngine.Graphics;

public interface IGraphicsBackend
{
    void BeginFrame();
    void Clear();
    void EndFrame();

    BackendTextureHandle CreateTexture(ImageData image);
    void DestroyTexture(BackendTextureHandle texture);

    void DrawTexture(TextureDrawCommand command);
    void DrawRectangle(RectangleDrawCommand command);
    void DrawTriangle(TriangleDrawCommand command);
    void DrawCircle(CircleDrawCommand command);
    void DrawLine(LineDrawCommand command);
    void DrawText(TextDrawCommand command);
}