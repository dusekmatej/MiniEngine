using MiniEngine.Graphics.Fonts;

namespace MiniEngine.Graphics;

public sealed class Graphics2D
{
    private readonly IGraphicsBackend _graphics;
    private readonly TextRenderer _textRenderer;

    public Graphics2D(
        IGraphicsBackend graphics,
        TextureManager textureManager,
        FontManager fontManager)
    {
        _graphics = graphics;

        _textRenderer = new TextRenderer(
            graphics,
            textureManager,
            fontManager
        );
    }

    public void DrawRectangle(
        float x,
        float y,
        float width,
        float height,
        EngineColor color)
    {
        _graphics.DrawRectangle(
            new RectangleDrawCommand(
                x,
                y,
                width,
                height,
                color
            )
        );
    }

    public void DrawText(
        FontAssetHandle font,
        string text,
        float x,
        float y,
        float height,
        EngineColor color)
    {
        _textRenderer.DrawText(
            font,
            text,
            x,
            y,
            height,
            color
        );
    }
}