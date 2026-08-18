namespace MiniEngine.Environment;

public sealed class TileDefinitionRegistry
{
    private readonly List<TileDefinition> _definitions = [];
    private readonly Dictionary<string, TileId> _mappedDefinitions = new();

    public TileId Register(TileDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (_mappedDefinitions.ContainsKey(definition.Key))
            throw new ArgumentException($"Tile definition '{definition.Key} is already registered.'");

        if (_definitions.Count >= ushort.MaxValue)
            throw new InvalidOperationException($"Tile definition registry is full. Max count is {ushort.MaxValue}.");

        var id = new TileId((ushort)(_definitions.Count + 1));

        _definitions.Add(definition);
        _mappedDefinitions.Add(definition.Key, id);

        return id;
    }

    public TileDefinition Get(TileId id)
    {
        if (!id.IsValid)
            throw new ArgumentException($"Tile definition id '{id.Value}' is invalid.");

        var index = id.Value - 1;

        if (index >= _definitions.Count)
            throw new ArgumentException($"Tile definition id '{id.Value}' is not registered.");

        return _definitions[index];
    } 

    public bool TryGetId(string key, out TileId id)
        => _mappedDefinitions.TryGetValue(key, out id);
}