using Silk.NET.Maths;

namespace Engine.Core;

public class GameLoop
{
    private const float TileWidth = 32f;
    private const float TileHeight = 16f;

    private readonly Time _time;
    private readonly List<(float X, float Y, float Width, float Height)> _tilesToDraw = [];

    public event Action<float>? Update;
    public event Action? Render;
    private Camera _camera;

    public IReadOnlyList<(float X, float Y, float Width, float Height)> TilesToDraw => _tilesToDraw;
    
    public GameLoop(Time time)
    {
        _camera = new Camera(new Vector2D<float>(0, 0));
         _time = time;
    }

    public void Tick()
    {

        _tilesToDraw.Clear();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                var world = new Vector3D<float>(x, y, 0);

                var screen = _camera.CalculateScreenPosition(world);

                DrawTile(screen.X, screen.Y);
            }
        }


        _time.Update();

        // Invoke the main Update & Render events
        Update?.Invoke(_time.DeltaTime);
        Render?.Invoke();
    }

    private void DrawTile(float x, float y)
    {
        _tilesToDraw.Add((x, y, TileWidth, TileHeight));
    }
}