namespace MiniEngine.Environment;

public sealed class TileDefinition
{
    public string Key { get; }
    public string Texture { get; set; }

    public TileDefinition(string key, string texture)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("TileDefinition: Key is null or empty!", nameof(key));

        if (string.IsNullOrWhiteSpace(texture))
            throw new ArgumentException("TileDefinition: Texture is null or empty!", texture);

        Key = key;
        Texture = texture;
    }
}