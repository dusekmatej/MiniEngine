using MiniEngine.Core;
using MiniEngine.Graphics;

namespace MiniEngine.AssetPipeline;

public sealed class FileAssetSource : IAssetSource
{
    private static readonly string[] SupportedExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];
    private static readonly char[] PathSeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
    private readonly string _assetRoot;

    public FileAssetSource(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _assetRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));

        if (!Directory.Exists(_assetRoot))
        {
            throw new DirectoryNotFoundException($"Asset root '{_assetRoot}' does not exist.");
        }

        RejectDirectoryLinks(_assetRoot);
    }

    public string NormalizeName(string name)
    {
        string relativePath = NormalizeRelativePath(name);
        string extension = Path.GetExtension(relativePath);

        bool hasExtension = extension.Length > 0;
        if (hasExtension && !SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new NotSupportedException($"Asset '{name}' has unsupported image extension '{extension}'.");
        }

        string relativeDirectory = Path.GetDirectoryName(relativePath) ?? string.Empty;
        string fullDirectoryPath = Path.Combine(_assetRoot, relativeDirectory);
        RejectDirectoryLinks(fullDirectoryPath);

        if (hasExtension)
        {
            if (!ImageFileExists(relativePath))
            {
                throw new FileNotFoundException(
                    $"Image asset '{name}' was not found under '{_assetRoot}'.",
                    Path.Combine(_assetRoot, relativePath));
            }

            return NormalizePathCase(relativePath);
        }

        foreach (string candidateExtension in SupportedExtensions)
        {
            string candidatePath = relativePath + candidateExtension;

            if (ImageFileExists(candidatePath))
            {
                return NormalizePathCase(candidatePath);
            }
        }

        string attemptedExtensions = string.Join(", ", SupportedExtensions);
        throw new FileNotFoundException(
            $"Image asset '{name}' was not found under '{_assetRoot}'. Tried {attemptedExtensions}.",
            name);
    }

    public ImageData LoadImage(string name)
    {
        string relativePath = NormalizeName(name);
        string fullPath = Path.Combine(_assetRoot, relativePath);

        try
        {
            return ImageLoader.Load(fullPath);
        }
        catch (Exception exception) when (
            exception is not IOException &&
            exception is not UnauthorizedAccessException &&
            exception is not OutOfMemoryException)
        {
            throw new InvalidDataException(
                $"Image asset '{name}' at '{fullPath}' could not be decoded as RGBA.",
                exception);
        }
    }

    private static string NormalizeRelativePath(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        string normalizedPath = name.Replace('\\', '/');

        if (Path.IsPathRooted(normalizedPath) || normalizedPath.Contains(':') || normalizedPath.Contains('\0'))
        {
            throw new ArgumentException("Asset identifiers must be relative paths.", nameof(name));
        }

        string[] pathSegments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var normalizedSegments = new List<string>(pathSegments.Length);

        foreach (string segment in pathSegments)
        {
            if (segment == "..")
            {
                throw new ArgumentException("Asset identifiers cannot traverse parent directories.", nameof(name));
            }

            if (segment == ".")
            {
                continue;
            }

            normalizedSegments.Add(segment);
        }

        if (normalizedSegments.Count == 0)
        {
            throw new ArgumentException("Asset identifier must name an image.", nameof(name));
        }

        return string.Join('/', normalizedSegments);
    }

    private bool ImageFileExists(string relativePath)
    {
        string fullPath = Path.Combine(_assetRoot, relativePath);
        var imageFile = new FileInfo(fullPath);
        RejectLink(imageFile);

        return imageFile.Exists;
    }

    private static string NormalizePathCase(string relativePath)
    {
        if (OperatingSystem.IsWindows())
        {
            return relativePath.ToUpperInvariant();
        }

        return relativePath;
    }

    private static void RejectDirectoryLinks(string directoryPath)
    {
        string? rootPath = Path.GetPathRoot(directoryPath);
        if (string.IsNullOrEmpty(rootPath))
        {
            throw new ArgumentException("Asset directory paths must be absolute.", nameof(directoryPath));
        }

        string pathWithoutRoot = directoryPath[rootPath.Length..];
        string[] pathSegments = pathWithoutRoot.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
        string currentPath = rootPath;

        foreach (string segment in pathSegments)
        {
            currentPath = Path.Combine(currentPath, segment);
            var directory = new DirectoryInfo(currentPath);
            RejectLink(directory);
        }
    }

    private static void RejectLink(FileSystemInfo entry)
    {
        bool isSymbolicLink = entry.LinkTarget is not null;
        bool isReparsePoint = entry.Exists && (entry.Attributes & FileAttributes.ReparsePoint) != 0;

        if (isSymbolicLink || isReparsePoint)
        {
            throw new UnauthorizedAccessException(
                $"Asset paths cannot contain symbolic links or reparse points: '{entry.FullName}'.");
        }
    }
}
