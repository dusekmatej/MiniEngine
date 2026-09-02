namespace MiniEngine.Graphics.Fonts;

public sealed class FontManager
{
    private readonly List<FontEntry> _fonts = new();
    private readonly Dictionary<string, int> _fontByName = new();

    public FontAssetHandle AddFont(string name, string path)
    {
        if (_fontByName.TryGetValue(name, out int existingIndex))
        {
            FontEntry existingFont = _fonts[existingIndex];

            return new FontAssetHandle(existingIndex);
        }

        if (!File.Exists(path))
            throw new FileNotFoundException($"Font file not found: {path}");

        byte[] data = File.ReadAllBytes(path);

        int index = _fonts.Count;

        var entry = new FontEntry
        {
            Name = name,
            Data = data
        };


        _fonts.Add(entry);
        _fontByName.Add(name, index);

        return new FontAssetHandle(index);
    }

    public ReadOnlyMemory<byte> GetData(FontAssetHandle handle)
    {
        return GetEntry(handle).Data;
    }

    private FontEntry GetEntry(FontAssetHandle handle)
    {
        if (handle.Index < 0 || handle.Index >= _fonts.Count)
            throw new ArgumentException("Invalid font handle: index out of range.");

        FontEntry entry = _fonts[handle.Index];

        return entry;
    }
}