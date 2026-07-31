using Database.Import;
using Engine.Graphics;
using Engine.Core;

namespace Game;

public class Game : IGame
{
    private Texture? _tileTexture;
    private Renderer? _renderer;

    public void Initialize(Renderer renderer)
    {
        _renderer = renderer;

        Console.WriteLine("Game: Initializing game...");

        TerrainImport.PopulateDatabase();

        var tile = global::Database.Database.Get<ImageData>("tile_000");

        if (tile != null)
        {
            Console.WriteLine(
                $"Game: Successfully retrieved image 'tile_000' from the database. " +
                $"Dimensions: {tile.Width}x{tile.Height}");

            _tileTexture = _renderer.CreateTexture(tile);
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
        if (_tileTexture == null)
            return;

        _renderer.DrawTexture(
            _tileTexture,
            0f,
            0f,
            1f,
            1f);
    }
}