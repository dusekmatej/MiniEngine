using System.Numerics;
using Silk.NET.Maths;

namespace MiniEngine.Graphics;

public class Camera
{
    private float _zoom = 1f;
    public Vector3 Position { get; set; }

    public float Zoom
    {
        get => _zoom;
        set => _zoom = Math.Clamp(value, 0.1f, 5f);
    }

    public Camera(Vector3 position)
    {
        Position = position;
    }

    public Vector2 WorldToScreen(Vector2 position)
    {
        return (position - new Vector2(Position.X, Position.Y)) * Zoom;
    }
}