using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct CircleDrawCommand(
    float X,
    float Y,
    float Radius,
    EngineColor Color,
    bool Filled = true,
    Vector2 Scale = default,
    int Layer = 0)
{
    public Vector2 Scale { get; init; } = Scale == default ? Vector2.One : Scale;
}