using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct CircleDrawCommand
{
    public float X { get; }
    public float Y { get; }
    public float Radius { get; }
    public EngineColor Color { get; }
    public Vector2 Scale { get; }
    public int Layer { get; }

    public CircleDrawCommand(
        float x,
        float y,
        float radius,
        EngineColor color,
        Vector2? scale = null,
        int layer = 0)
    {
        X = x;
        Y = y;
        Radius = radius;
        Color = color;
        Scale = scale ?? Vector2.One;
        Layer = layer;
    }
}