using Database;
using Engine.Core;
using StbImageSharp;

namespace Game;

public class Game : IGame
{
    public void Initialize()
    {
        Console.WriteLine("Game: Initializing game...");
        AutoLoader.PopulateDatabase();

        var tile = global::Database.Database.Get<ImageResult>("tile_000");
        
        if (tile != null)
        {
            Console.WriteLine($"Game: Successfully retrieved image 'tile_000' from the database. Dimensions: {tile.Width}x{tile.Height}");
        }
        else
        {
            Console.WriteLine("Game: Failed to retrieve image 'tile_000' from the database.");
        }
    }

    public void Update(float deltaTime)
    {
        // Console.WriteLine($"Game Update: {deltaTime:F10}");
    }

    public void Render()
    {
    }
}