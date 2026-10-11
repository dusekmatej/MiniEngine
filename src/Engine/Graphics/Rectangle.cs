using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct Rectangle(
    float X,
    float Y,
    float Width,
    float Height)
{
    public bool Contains(Vector2 point)
    {
        return point.X >= X &&
            point.X <= X + Width &&
            point.Y >= Y &&
            point.Y <= Y + Height;
    }
}