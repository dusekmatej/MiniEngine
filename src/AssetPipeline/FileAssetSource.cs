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

        _targetLocation = targetLocation;
    }

    public ImageData LoadImage(string name)
    {
        string path = Path.Combine(_targetLocation, name);

        if (!Path.HasExtension(path))
            path += ".png";

        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        return ImageLoader.Load(path);
    }
}

