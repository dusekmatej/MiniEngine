using MiniEngine.Core;
using MiniEngine.Graphics;

namespace MiniEngine.AssetPipeline;

public sealed class FileAssetSource : IAssetSource
{
    private static readonly string[] Extensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];
    private readonly string _root;

    public FileAssetSource(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        if (!Directory.Exists(_root))
            throw new DirectoryNotFoundException($"Asset root '{_root}' does not exist.");
        RejectLinks(_root);
    }

    public string NormalizeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        string relative = name.Replace('\\', '/');
        if (Path.IsPathRooted(relative) || relative.Contains(':') || relative.Contains('\0'))
            throw new ArgumentException("Asset identifiers must be relative paths.", nameof(name));

        string[] segments = relative.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Contains(".."))
            throw new ArgumentException("Asset identifiers cannot traverse parent directories.", nameof(name));
        relative = string.Join('/', segments.Where(segment => segment != "."));
        if (relative.Length == 0)
            throw new ArgumentException("Asset identifier must name an image.", nameof(name));

        string extension = Path.GetExtension(relative);
        if (extension.Length > 0)
        {
            if (!Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                throw new NotSupportedException($"Asset '{name}' has unsupported image extension '{extension}'.");
            return ResolveExisting(relative, name);
        }

        foreach (string candidateExtension in Extensions)
        {
            string candidate = relative + candidateExtension;
            string path = Path.Combine(_root, candidate);
            RejectLinks(path);
            if (File.Exists(path))
                return CanonicalName(candidate);
        }

        throw new FileNotFoundException($"Image asset '{name}' was not found under '{_root}'. Tried {string.Join(", ", Extensions)}.", name);
    }

    public ImageData LoadImage(string name)
    {
        string path = Path.Combine(_root, NormalizeName(name));
        try
        {
            return ImageLoader.Load(path);
        }
        catch (Exception exception) when (exception is not IOException && exception is not UnauthorizedAccessException)
        {
            throw new InvalidDataException($"Image asset '{name}' at '{path}' could not be decoded as RGBA.", exception);
        }
    }

    private string ResolveExisting(string relative, string name)
    {
        string path = Path.Combine(_root, relative);
        RejectLinks(path);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Image asset '{name}' was not found under '{_root}'.", path);
        return CanonicalName(relative);
    }

    private static string CanonicalName(string relative)
        => OperatingSystem.IsWindows() ? relative.ToUpperInvariant() : relative;

    private static void RejectLinks(string path)
    {
        string current = Path.GetPathRoot(path)!;
        foreach (string segment in path[current.Length..].Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries))
        {
            current = Path.Combine(current, segment);
            FileSystemInfo info = Directory.Exists(current) ? new DirectoryInfo(current) : new FileInfo(current);
            if (info.LinkTarget is not null || (info.Exists && (info.Attributes & FileAttributes.ReparsePoint) != 0))
                throw new UnauthorizedAccessException($"Asset paths cannot contain symbolic links or reparse points: '{current}'.");
        }
    }
}
