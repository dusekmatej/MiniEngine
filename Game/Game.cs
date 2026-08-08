using MiniEngine.Database.Import;
using MiniEngine.Graphics;
using MiniEngine.Core;

namespace MiniEngine.Game;

public class Game : IGame
{
    private BackendTextureHandle? _tileTexture;
    private IGraphicsBackend? _graphics;

    public void Initialize(IGraphicsBackend graphics)
    {
        _graphics = graphics;

        Console.WriteLine("Game: Initializing game...");

        TerrainImport.PopulateDatabase();

        var tile = global::MiniEngine.Database.Database.Get<ImageData>("tile_000");

        if (tile != null)
        {
            Console.WriteLine(
                $"Game: Successfully retrieved image 'tile_000' from the database. " +
                $"Dimensions: {tile.Width}x{tile.Height}");

            _tileTexture = _graphics.CreateTexture(tile);
        }
        else
        {
            Console.WriteLine(
                "Game: Failed to retrieve image 'tile_000' from the database.");
        }
    }

    public void Update(float deltaTime)
    {
        // Console.WriteLine($"Game Update: {deltaTime:F10}");
    }

    public void Render()
    {
        if (_tileTexture is null || _graphics is null)
            return;

        _graphics.DrawTexture(
            _tileTexture.Value,
            0f,
            0f,
            1f,
            1f);
    }
}