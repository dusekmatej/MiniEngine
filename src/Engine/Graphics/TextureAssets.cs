using MiniEngine.Core;

namespace MiniEngine.Graphics;

public class TextureAssets : IDisposable
{
    private sealed class TextureEntry
    {
        public required string Name { get; init; }
        public required bool Generated { get; init; }
        public int Generation { get; init; } = 1;
        public ImageData? Image { get; set; }
        public BackendTextureHandle? BackendHandle { get; set; }
    }

    private readonly IGraphicsBackend _graphics;
    private readonly IAssetSource? _assetSource;
    private readonly bool _retainSourceImages;
    private readonly int _ownerThread = System.Environment.CurrentManagedThreadId;
    private readonly List<TextureEntry?> _textures = new();
    private readonly Dictionary<string, int> _textureByName = new(StringComparer.Ordinal);
    private bool _disposed;

    public int RegisteredCount => _textureByName.Count;
    public int UploadedCount { get; private set; }
    public int RetainedImageCount { get; private set; }

    public TextureAssets(
        IGraphicsBackend graphics,
        IAssetSource? assetSource = null,
        bool retainSourceImages = false)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        _graphics = graphics;
        _assetSource = assetSource;
        _retainSourceImages = retainSourceImages;
    }

    public TextureAssetHandle Load(string name)
    {
        EnsureUsable();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_textureByName.TryGetValue(name, out int existingIndex))
        {
            return HandleAt(existingIndex);
        }

        if (_assetSource is null)
        {
            throw new InvalidOperationException("No asset source was provided.");
        }

        string normalizedName = _assetSource.NormalizeName(name);
        if (_textureByName.TryGetValue(normalizedName, out existingIndex))
        {
            return HandleAt(existingIndex);
        }

        var entry = new TextureEntry
        {
            Name = normalizedName,
            Generated = false
        };

        return Register(entry);
    }

    public TextureAssetHandle Load(string name, ImageData imageData)
    {
        EnsureUsable();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(imageData);

        if (_textureByName.TryGetValue(name, out int existingIndex))
        {
            return HandleAt(existingIndex);
        }

        var entry = new TextureEntry
        {
            Name = name,
            Image = imageData,
            Generated = true
        };

        TextureAssetHandle handle = Register(entry);
        RetainedImageCount++;
        return handle;
    }

    public BackendTextureHandle GetBackendHandle(TextureAssetHandle handle)
    {
        TextureEntry entry = GetEntry(handle);
        if (entry.BackendHandle is BackendTextureHandle cachedHandle)
        {
            return cachedHandle;
        }

        ImageData? image = entry.Image;
        if (image is null)
        {
            if (_assetSource is null)
            {
                throw new InvalidOperationException("No asset source was provided.");
            }

            image = _assetSource.LoadImage(entry.Name);
        }

        BackendTextureHandle backendHandle = _graphics.CreateTexture(image);
        entry.BackendHandle = backendHandle;
        UploadedCount++;

        if (_retainSourceImages && entry.Image is null)
        {
            entry.Image = image;
            RetainedImageCount++;
        }

        return backendHandle;
    }

    public void Preload(TextureAssetHandle handle) => GetBackendHandle(handle);

    public void Preload(IEnumerable<TextureAssetHandle> handles)
    {
        ArgumentNullException.ThrowIfNull(handles);

        foreach (TextureAssetHandle handle in handles)
        {
            Preload(handle);
        }
    }

    public void Unload(TextureAssetHandle handle)
    {
        TextureEntry entry = GetEntry(handle);
        UnloadEntry(entry);
    }

    public void Reload(TextureAssetHandle handle)
    {
        TextureEntry entry = GetEntry(handle);
        UnloadEntry(entry);

        if (!entry.Generated && entry.Image is not null)
        {
            entry.Image = null;
            RetainedImageCount--;
        }

        Preload(handle);
    }

    public void Remove(TextureAssetHandle handle)
    {
        TextureEntry entry = GetEntry(handle);
        UnloadEntry(entry);

        if (entry.Image is not null)
        {
            RetainedImageCount--;
        }

        _textureByName.Remove(entry.Name);
        _textures[handle.Index] = null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        EnsureUsable();

        foreach (TextureEntry? entry in _textures)
        {
            if (entry is not null)
            {
                UnloadEntry(entry);
            }
        }

        _textures.Clear();
        _textureByName.Clear();
        RetainedImageCount = 0;
        _disposed = true;
    }

    private void UnloadEntry(TextureEntry entry)
    {
        if (entry.BackendHandle is not BackendTextureHandle backendHandle)
        {
            return;
        }

        _graphics.DestroyTexture(backendHandle);
        entry.BackendHandle = null;
        UploadedCount--;
    }

    private TextureAssetHandle Register(TextureEntry entry)
    {
        int index = _textures.Count;
        _textures.Add(entry);
        _textureByName.Add(entry.Name, index);
        return new TextureAssetHandle(index, entry.Generation);
    }

    private TextureAssetHandle HandleAt(int index)
    {
        TextureEntry? entry = _textures[index];
        if (entry is null)
        {
            throw new InvalidOperationException("Texture registration no longer exists.");
        }

        return new TextureAssetHandle(index, entry.Generation);
    }

    private TextureEntry GetEntry(TextureAssetHandle handle)
    {
        EnsureUsable();

        if (handle.Index < 0 || handle.Index >= _textures.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(handle), "Texture handle index is invalid.");
        }

        TextureEntry? entry = _textures[handle.Index];
        if (entry is null || entry.Generation != handle.Generation)
        {
            throw new InvalidOperationException("Texture handle is no longer valid.");
        }

        return entry;
    }

    private void EnsureUsable()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (System.Environment.CurrentManagedThreadId != _ownerThread)
        {
            throw new InvalidOperationException("Texture assets must be used on their owning graphics thread.");
        }
    }
}
