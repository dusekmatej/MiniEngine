using MiniEngine;
using MiniEngine.AssetPipeline;
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

        var graphicsFactory = new BackendFactory();
        var assetSource = new FileAssetSource(Path.Combine(AppContext.BaseDirectory, "Assets"));
        Engine engine = new Engine(game, graphicsFactory, assetSource);

        engine.Run();
    }
}



