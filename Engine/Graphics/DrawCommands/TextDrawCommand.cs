using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct TextDrawCommand
{
    public BackendTextureHandle Texture { get; }
    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }
    public EngineColor Color { get; }
    public float Rotation { get; }
    public Vector2 Scale { get; }
    public int Layer { get; }

    public TextDrawCommand(
        BackendTextureHandle texture,
        float x,
        float y,
        float width,
        float height,
        EngineColor color)
        : this(
            texture,
            x,
            y,
            width,
            height,
            color,
            0f,
            Vector2.One,
            0)
    {
    }

    public TextDrawCommand(
        BackendTextureHandle texture,
        float x,
        float y,
        float width,
        float height,
        EngineColor color,
        float rotation,
        Vector2 scale,
        int layer)
    {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
        Rotation = rotation;
        Scale = scale;
        Layer = layer;
    }
}