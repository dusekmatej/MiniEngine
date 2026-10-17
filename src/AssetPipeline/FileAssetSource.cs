using MiniEngine.Core;
using MiniEngine.Graphics;

namespace MiniEngine.AssetPipeline;

public sealed class FileAssetSource : IAssetSource
{
    private readonly string _targetLocation;

    public FileAssetSource(string targetLocation)
    {
        if (!Directory.Exists(targetLocation))
            throw new DirectoryNotFoundException($"Route invalid: {targetLocation}");

        _targetLocation = Path.GetFullPath(targetLocation);
    }

    public ImageData LoadImage(string name)
    {
        string path = ResolvePath(name);

        if (!Path.HasExtension(path))
            path += ".png";

        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        return ImageLoader.Load(path);
    }

    public IReadOnlyList<string> GetImageNames(string path)
    {
        string relativePath = path.Replace('\\', '/').TrimEnd('/');
        bool recursive = relativePath.EndsWith("/*", StringComparison.Ordinal);

        if (recursive)
            relativePath = relativePath[..^2];
        else if (relativePath == "*")
        {
            relativePath = string.Empty;
            recursive = true;
        }

        if (relativePath.Contains('*') || relativePath.Contains('?'))
            throw new ArgumentException("Only a trailing /* wildcard is supported.", nameof(path));

        string directory = ResolvePath(relativePath);

        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Directory not found: {directory}");

        var imageNames = new List<string>();
        CollectImageNames(directory, recursive, imageNames);
        return imageNames;
    }

    private void CollectImageNames(string directory, bool recursive, List<string> imageNames)
    {
        string[] files = Directory.GetFiles(directory);
        Array.Sort(files, StringComparer.Ordinal);
        bool hasImages = false;

        foreach (string file in files)
        {
            string extension = Path.GetExtension(file).ToLowerInvariant();

            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".tga":
                case ".psd":
                case ".hdr":
                case ".pic":
                case ".ppm":
                case ".pgm":
                    break;
                default:
                    continue;
            }

            string imageName = Path.GetRelativePath(_targetLocation, file).Replace('\\', '/');

            if (Path.GetExtension(file) == ".png")
                imageName = imageName[..^4];

            imageNames.Add(imageName);
            hasImages = true;
        }

        if (hasImages && !recursive)
            return;

        string[] directories = Directory.GetDirectories(directory);
        Array.Sort(directories, StringComparer.Ordinal);

        foreach (string subdirectory in directories)
        {
            if ((File.GetAttributes(subdirectory) & FileAttributes.ReparsePoint) != 0)
                continue;

            CollectImageNames(subdirectory, recursive, imageNames);
        }
    }

    private string ResolvePath(string path)
    {
        string relativePath = path.Replace('\\', '/').TrimStart('/');

        if (relativePath.Equals("Assets", StringComparison.OrdinalIgnoreCase))
            relativePath = string.Empty;
        else if (relativePath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath[7..];

        string fullPath = Path.GetFullPath(Path.Combine(_targetLocation, relativePath));
        string pathFromRoot = Path.GetRelativePath(_targetLocation, fullPath);

        if (pathFromRoot == ".." || pathFromRoot.StartsWith("../", StringComparison.Ordinal)
            || pathFromRoot.StartsWith("..\\", StringComparison.Ordinal) || Path.IsPathRooted(pathFromRoot))
        {
            throw new ArgumentException("Asset paths must stay inside the asset directory.", nameof(path));
        }

        return fullPath;
    }
}

