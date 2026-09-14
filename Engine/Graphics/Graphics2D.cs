using MiniEngine.Graphics.Fonts;
using System.Numerics;

namespace MiniEngine.Graphics;

public sealed class Graphics2D
{
    private readonly IGraphicsBackend _graphics;
    private readonly TextureAssets _textureAssets;
    private readonly TextRenderer _textRenderer;
    private readonly List<QueuedCommand> _commands = new();

    public Vector2 MousePosition { get; private set; }
    public bool IsLeftMouseButtonPressed { get; private set; }

    private sealed class QueuedCommand
    {
        public RectangleDrawCommand? Rectangle { get; init; }
        public TriangleDrawCommand? Triangle { get; init; }
        public CircleDrawCommand? Circle { get; init; }
        public LineDrawCommand? Line { get; init; }
        public TextureDrawCommand? Texture { get; init; }
        public TextDrawCommand? Text { get; init; }
    }

    public Graphics2D(
        IGraphicsBackend graphics,
        TextureAssets textureAssets,
        FontManager fontManager)
    {
        _graphics = graphics;
        _textureAssets = textureAssets;

        _textRenderer = new TextRenderer(
            QueueText,
            textureAssets,
            fontManager
        );
    }

    public void SetMouseState(
        Vector2 mousePosition,
        bool isLeftMouseButtonPressed)
    {
        MousePosition = mousePosition;
        IsLeftMouseButtonPressed = isLeftMouseButtonPressed;
    }

    public void Flush()
    {
        foreach (QueuedCommand queuedCommand in _commands)
        {
            if (queuedCommand.Rectangle is RectangleDrawCommand rectangle)
                _graphics.DrawRectangle(rectangle);
            else if (queuedCommand.Triangle is TriangleDrawCommand triangle)
                _graphics.DrawTriangle(triangle);
            else if (queuedCommand.Circle is CircleDrawCommand circle)
                _graphics.DrawCircle(circle);
            else if (queuedCommand.Line is LineDrawCommand line)
                _graphics.DrawLine(line);
            else if (queuedCommand.Texture is TextureDrawCommand texture)
                _graphics.DrawTexture(texture);
            else if (queuedCommand.Text is TextDrawCommand text)
                _graphics.DrawText(text);
        }

        _commands.Clear();
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
        QueueRectangle(
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

    public void DrawRectangle(
        Rectangle bounds,
        EngineColor color,
        int layer = 0)
    {
        DrawRectangle(
            bounds.X,
            bounds.Y,
            bounds.Width,
            bounds.Height,
            color,
            layer: layer
        );
    }

    public void DrawRectangleOutline(
        Rectangle bounds,
        EngineColor color,
        float thickness = 1f,
        int layer = 0)
    {
        DrawLine(
            new Vector2(bounds.X, bounds.Y),
            new Vector2(bounds.X + bounds.Width, bounds.Y),
            color,
            thickness,
            layer
        );
        DrawLine(
            new Vector2(bounds.X + bounds.Width, bounds.Y),
            new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height),
            color,
            thickness,
            layer
        );
        DrawLine(
            new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height),
            new Vector2(bounds.X, bounds.Y + bounds.Height),
            color,
            thickness,
            layer
        );
        DrawLine(
            new Vector2(bounds.X, bounds.Y + bounds.Height),
            new Vector2(bounds.X, bounds.Y),
            color,
            thickness,
            layer
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

        QueueTexture(
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
        QueueTriangle(
            new TriangleDrawCommand(
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

    public void DrawCircle(
        float x,
        float y,
        float radius,
        EngineColor color,
        bool filled = true,
        Vector2? scale = null,
        int layer = 0)
    {
        QueueCircle(
            new CircleDrawCommand(
                x,
                y,
                radius,
                color,
                filled,
                scale ?? Vector2.One,
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
        DrawLine(
            new Vector2(startX, startY),
            new Vector2(endX, endY),
            color,
            thickness,
            layer
        );
    }

    public void DrawLine(
        Vector2 start,
        Vector2 end,
        EngineColor color,
        float thickness = 1f,
        int layer = 0)
    {
        QueueLine(
            new LineDrawCommand(
                start.X,
                start.Y,
                end.X,
                end.Y,
                thickness,
                color,
                layer
            )
        );
    }

    public bool DrawButton(
        Rectangle bounds,
        string text,
        EngineColor backgroundColor,
        EngineColor textColor,
        FontAssetHandle fontHandle)
    {
        bool isHovered = bounds.Contains(MousePosition);
        bool isPressed = IsLeftMouseButtonPressed && isHovered;

        EngineColor buttonColor = isPressed
            ? EngineColor.FromNormalized(0.08f, 0.18f, 0.35f)
            : isHovered
                ? EngineColor.FromNormalized(0.15f, 0.35f, 0.75f)
                : backgroundColor;

        DrawRectangle(bounds, buttonColor, layer: 3);
        DrawRectangleOutline(bounds, textColor, 0.02f, layer: 4);

        DrawText(
            fontHandle,
            text,
            bounds.X + bounds.Width * 0.20f,
            bounds.Y + bounds.Height * 0.27f,
            bounds.Height * 0.40f,
            textColor,
            layer: 5
        );

        return isPressed;
    }

    public void DrawLabel(
        Vector2 position,
        string text,
        EngineColor color,
        FontAssetHandle fontHandle)
    {
        DrawText(
            fontHandle,
            text,
            position.X,
            position.Y,
            0.08f,
            color
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

    private void QueueRectangle(RectangleDrawCommand command)
        => _commands.Add(new QueuedCommand { Rectangle = command });

    private void QueueTexture(TextureDrawCommand command)
        => _commands.Add(new QueuedCommand { Texture = command });

    private void QueueTriangle(TriangleDrawCommand command)
        => _commands.Add(new QueuedCommand { Triangle = command });

    private void QueueCircle(CircleDrawCommand command)
        => _commands.Add(new QueuedCommand { Circle = command });

    private void QueueLine(LineDrawCommand command)
        => _commands.Add(new QueuedCommand { Line = command });

    private void QueueText(TextDrawCommand command)
        => _commands.Add(new QueuedCommand { Text = command });
}