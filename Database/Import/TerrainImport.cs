using MiniEngine.Engine.Core;
using MiniEngine.AssetPipeline;
using System.Linq;

namespace MiniEngine.Database.Import;

public static class TerrainImport
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];
    private static readonly string DatabaseDirectory = Path.Combine(AppContext.BaseDirectory, "Database", "Terrain");

    public static void PopulateDatabase()
    {
        Console.WriteLine($"TerrainImport: Loading images from directory '{DatabaseDirectory}'...");

        if (!Directory.Exists(DatabaseDirectory))
            throw new Exception($"TerrainImport error: Database directory '{DatabaseDirectory}' does not exist.");

        foreach (var file in Directory.GetFiles(DatabaseDirectory)
            .Where(file => AllowedExtensions.Contains(Path.GetExtension(file))))
        {
            var imageData = ImageLoader.Load(file);

            Database.Import(Path.GetFileNameWithoutExtension(file), imageData);
        }
    }
}