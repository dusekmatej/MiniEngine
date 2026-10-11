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
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Zoom must be greater than zero.");

            _zoom = value;
        }
    }

    public Camera(Vector3 position)
    {
        Position = position;
    }
}