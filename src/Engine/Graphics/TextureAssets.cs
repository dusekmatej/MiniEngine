using MiniEngine.Core;

namespace MiniEngine.Graphics;

public class TextureAssets
{
    private sealed class TextureEntry
    {
        public required string Name { get; init; }
        public required ImageData Image { get; init; }

        public required int Generation { get; init; }

        public BackendTextureHandle? BackendHandle { get; set; }
    }

    private readonly IGraphicsBackend _graphics;
    private readonly IAssetSource? _assetSource;

    private readonly List<TextureEntry> _textures = new();
    private readonly Dictionary<string, int> _textureByName = new();

    public TextureAssets(
        IGraphicsBackend graphics,
        IAssetSource? assetSource = null)
    {
        _graphics = graphics;
        _assetSource = assetSource;
    }

    public TextureAssetHandle Load(string name)
    {
        if (_textureByName.TryGetValue(name, out int existingIndex))
        {
            TextureEntry existingEntry = _textures[existingIndex];

            return new TextureAssetHandle(existingIndex, existingEntry.Generation);
        }

        if (_assetSource is null)
            throw new InvalidOperationException("No asset source was provided.");

        return Load(name, _assetSource.LoadImage(name));
    }

    public TextureAssetHandle Load(string name, ImageData imageData)
    {
        if (_textureByName.TryGetValue(name, out int existingIndex))
        {
            TextureEntry existingEntry = _textures[existingIndex];

            return new TextureAssetHandle(existingIndex, existingEntry.Generation);
        }

        int index = _textures.Count;

        var entry = new TextureEntry
        {
            Name = name,
            Image = imageData,
            Generation = 1
        };

        _textures.Add(entry);
        _textureByName.Add(name, index);

        return new TextureAssetHandle(index, entry.Generation);
    }

    public BackendTextureHandle GetBackendHandle(TextureAssetHandle handle)
    {
        TextureEntry entry = GetEntry(handle);

        if (entry.BackendHandle is null)
            entry.BackendHandle = _graphics.CreateTexture(entry.Image);

        return entry.BackendHandle.Value;
    }

    private TextureEntry GetEntry(TextureAssetHandle handle)
    {
        if (handle.Index < 0 || handle.Index >= _textures.Count)
            throw new ArgumentOutOfRangeException();

        TextureEntry entry = _textures[handle.Index];

        if (entry.Generation != handle.Generation)
            throw new InvalidOperationException("Texture handle is no longer valid.");

        return entry;
    }
}