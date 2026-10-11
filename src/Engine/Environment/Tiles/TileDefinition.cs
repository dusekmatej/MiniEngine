namespace MiniEngine.Environment;

public sealed class TileDefinition
{
    public string Key { get; }

    public TileDefinition(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("The tile definition is null or empty.", nameof(key));

        Key = key;
    }
}