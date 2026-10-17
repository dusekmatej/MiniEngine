using MiniEngine.AssetPipeline;
using MiniEngine.OpenGL.Core;
using System.Reflection;

namespace MiniEngine.Bootstrap;

public class Program
{
    public static void Main(string[] args)
    {
        Game.Game game = new Game.Game();
        BuildEngine(game).Run();
    }

    private static void DisplayVersion()
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

        Console.WriteLine("----------------- MiniEngine -----------------");
        Console.WriteLine($"Initializing MiniEngine on version: {version}");
        Console.WriteLine("----------------------------------------------");
    }

    private static Engine BuildEngine(Game.Game _g)
    {     
        var assetRoot = Path.Combine(AppContext.BaseDirectory, "Assets");
        var assetSource = new FileAssetSource(assetRoot);
        
        var graphicsFactory = new BackendFactory();
        
        return new Engine(_g, graphicsFactory, assetSource);
    }
}



