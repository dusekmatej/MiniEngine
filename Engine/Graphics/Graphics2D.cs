using MiniEngine.Graphics.Fonts;
using System.Numerics;

namespace MiniEngine.Graphics;

public sealed class Graphics2D
{
    private readonly IGraphicsBackend _graphics;
    private readonly TextureAssets _textureAssets;
    private readonly TextRenderer _textRenderer;

    public Graphics2D(
        IGraphicsBackend graphics,
        TextureAssets textureAssets,
        FontManager fontManager)
    {
        _graphics = graphics;
        _textureAssets = textureAssets;

        _textRenderer = new TextRenderer(
            graphics,
            textureAssets,
            fontManager
        );
    }

    public void DrawRectangle(
        float x,
        float y,
        float width,
        float height,
        EngineColor color,
        float rotation = 0f,
        Vector2? scale = null,
        int layer = 0)
    {
        _graphics.DrawRectangle(
            new RectangleDrawCommand(
                x,
                y,
                width,
                height,
                color,
                rotation,
                scale ?? Vector2.One,
                layer
            )
        );
    }

    public void DrawTexture(
        TextureAssetHandle texture,
        float x,
        float y,
        float width,
        float height,
        EngineColor tint,
        float rotation = 0f,
        Vector2? scale = null,
        int layer = 0)
    {
        BackendTextureHandle backendTexture =
            _textureAssets.GetBackendHandle(texture);

        _graphics.DrawTexture(
            new TextureDrawCommand(
                backendTexture,
                x,
                y,
                width,
                height,
                rotation,
                scale ?? Vector2.One,
                tint,
                layer
            )
        );
    }

    public void DrawSprite(
        TextureAssetHandle texture,
        float x,
        float y,
        float width,
        float height,
        EngineColor tint,
        float rotation = 0f,
        Vector2? scale = null,
        int layer = 0)
    {
        DrawTexture(
            texture,
            x,
            y,
            width,
            height,
            tint,
            rotation,
            scale,
            layer
        );
    }

    public void DrawTriangle(
        float x,
        float y,
        float width,
        float height,
        EngineColor color,
        float rotation = 0f,
        Vector2? scale = null,
        int layer = 0)
    {
        _graphics.DrawTriangle(
            new TriangleDrawCommand(
                x,
                y,
                width,
                height,
                color,
                rotation,
                scale,
                layer
            )
        );
    }

    public void DrawCircle(
        float x,
        float y,
        float radius,
        EngineColor color,
        Vector2? scale = null,
        int layer = 0)
    {
        _graphics.DrawCircle(
            new CircleDrawCommand(
                x,
                y,
                radius,
                color,
                scale,
                layer
            )
        );
    }

    public void DrawLine(
        float startX,
        float startY,
        float endX,
        float endY,
        float thickness,
        EngineColor color,
        int layer = 0)
    {
        _graphics.DrawLine(
            new LineDrawCommand(
                startX,
                startY,
                endX,
                endY,
                thickness,
                color,
                layer
            )
        );
    }

    public void DrawText(
        FontAssetHandle font,
        string text,
        float x,
        float y,
        float height,
        EngineColor color,
        float rotation = 0f,
        Vector2? scale = null,
        int layer = 0)
    {
        _textRenderer.DrawText(
            font,
            text,
            x,
            y,
            height,
            color,
            rotation,
            scale ?? Vector2.One,
            layer
        );
    }
}