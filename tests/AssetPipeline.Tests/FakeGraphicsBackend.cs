using MiniEngine.Core;
using MiniEngine.Graphics;

namespace MiniEngine.Tests;

internal sealed class FakeGraphicsBackend : IGraphicsBackend
{
    public int CreateCount { get; private set; }
    public int DestroyCount { get; private set; }
    public int TextureDrawCount { get; private set; }
    public int TextDrawCount { get; private set; }
    public bool FailUpload { get; set; }
    public ImageData? LastImage { get; private set; }
    public HashSet<BackendTextureHandle> LiveTextures { get; } = new();

    public BackendTextureHandle CreateTexture(ImageData image)
    {
        if (FailUpload)
            throw new InvalidOperationException("Upload failed.");
        LastImage = image;
        var handle = new BackendTextureHandle(++CreateCount);
        LiveTextures.Add(handle);
        return handle;
    }

    public void DestroyTexture(BackendTextureHandle texture)
    {
        if (!LiveTextures.Remove(texture))
            throw new InvalidOperationException("Texture was already destroyed.");
        DestroyCount++;
    }

    public void BeginFrame() { }
    public void Clear() { }
    public void EndFrame() { }
    public void DrawTexture(TextureDrawCommand command)
    {
        if (!LiveTextures.Contains(command.Texture))
            throw new InvalidOperationException("Missing texture.");
        TextureDrawCount++;
    }
    public void DrawRectangle(RectangleDrawCommand command) { }
    public void DrawTriangle(TriangleDrawCommand command) { }
    public void DrawCircle(CircleDrawCommand command) { }
    public void DrawLine(LineDrawCommand command) { }
    public void DrawText(TextDrawCommand command)
    {
        if (!LiveTextures.Contains(command.Texture))
            throw new InvalidOperationException("Missing glyph texture.");
        TextDrawCount++;
    }
}
