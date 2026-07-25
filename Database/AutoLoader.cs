using StbImageSharp;
using System.Linq;

namespace Database;

public static class AutoLoader
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];
    private static readonly string DatabaseDirectory = Path.Combine(AppContext.BaseDirectory, "Database", "Terrain");

    public static void PopulateDatabase()
    {
        Console.WriteLine($"AutoLoader: Loading images from directory '{DatabaseDirectory}'...");

        if (!Directory.Exists(DatabaseDirectory))
            throw new Exception($"AutoLoader error: Database directory '{DatabaseDirectory}' does not exist.");

        foreach (var file in Directory.GetFiles(DatabaseDirectory)
                                    .Where(file => AllowedExtensions.Contains(Path.GetExtension(file))))
        {
            Database.Import<ImageResult>(
                Path.GetFileNameWithoutExtension(file),
                LoadImage(file));
        }
    }

    private static ImageResult LoadImage(string path)
    {
        return ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
    }
}