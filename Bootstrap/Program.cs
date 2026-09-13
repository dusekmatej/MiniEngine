using MiniEngine;
using MiniEngine.Database;
using MiniEngine.Database.Import;
using MiniEngine.OpenGL.Core;
using System.Reflection;

namespace MiniEngine.Bootstrap;

public class Program
{
    public static void Main(string[] args)
    {
        Game.Game game = new Game.Game();
        CreateEngine(game);
    }

    private static void DisplayVersion()
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

        Console.WriteLine("----------------- MiniEngine -----------------");
        Console.WriteLine($"Initializing MiniEngine on version: {version}");
        Console.WriteLine("----------------------------------------------");
    }

    private static void CreateEngine(Game.Game game)
    {
        DisplayVersion();

        TerrainImport.PopulateDatabase();

        var graphicsFactory = new BackendFactory();
        var assetSource = new DatabaseAssetSource();
        Engine engine = new Engine(game, graphicsFactory, assetSource);

        engine.Run();
    }
}



