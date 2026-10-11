using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct TriangleDrawCommand(
    float X,
    float Y,
    float Width,
    float Height,
    EngineColor Color,
    float Rotation = 0f,
    Vector2 Scale = default,
    int Layer = 0)
{
    public Vector2 Scale { get; init; } = Scale == default ? Vector2.One : Scale;
}