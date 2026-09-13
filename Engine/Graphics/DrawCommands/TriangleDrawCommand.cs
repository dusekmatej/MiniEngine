using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct TriangleDrawCommand
{
    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }
    public EngineColor Color { get; }
    public float Rotation { get; }
    public Vector2 Scale { get; }
    public int Layer { get; }

    public TriangleDrawCommand(
        float x,
        float y,
        float width,
        float height,
        EngineColor color,
        float rotation = 0f,
        Vector2? scale = null,
        int layer = 0)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
        Rotation = rotation;
        Scale = scale ?? Vector2.One;
        Layer = layer;
    }
}