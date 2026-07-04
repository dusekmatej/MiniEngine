using Silk.NET.Maths;

public class Camera
{
    public Vector2D<float> Position { get; set; }
    public float Zoom { get; set; }

    public Camera(Vector2D<float> position)
    {
        Position = position; 
    }

    public Vector2D<float> CalculateScreenPosition(Vector3D<float> worldPosition)
    {
        var screenX = (worldPosition.X - worldPosition.Y) * 32f;
        var screenY = (worldPosition.X + worldPosition.Y) * 16f - worldPosition.Z * 32f;

        screenX -= Position.X;
        screenY -= Position.Y;

        screenX *= Zoom;
        screenY *= Zoom;

        return new Vector2D<float>(screenX, screenY);
    }
}