namespace MiniEngine.Environment;

public class TerrainGenerator
{
    private readonly Perlin _perlin;
    private readonly TileDefinitionRegistry _definitions;

    private readonly List<TerrainRule> _rules = new();

    public TerrainGenerator(int seed, TileDefinitionRegistry definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        _perlin = new Perlin(seed);
        _definitions = definitions;
    }

    public void Add(string tile, int min, int max)
    {
        if (min < 0 || max > 100 || min > max)
            throw new ArgumentOutOfRangeException($"TerrainGenerator: Invalid min/max values! Min: {min}, Max: {max}");

        if (!_definitions.TryGetId(tile, out var tileId))
            throw new ArgumentException($"TerrainGenerator: Invalid tile name! {tile}, or not registered!");

        foreach (TerrainRule rule in _rules)
        {
            if (min < rule.Max && max > rule.Min)
                throw new ArgumentException($"TerrainGenerator: Overlapping min/max values! Min: {min}, Max: {max}");
        }

        _rules.Add(new TerrainRule(tileId, min, max));
    }

    public void Generate(Chunk chunk)
    {
        ArgumentNullException.ThrowIfNull(chunk);

        if (_rules.Count == 0)
            throw new InvalidOperationException("TerrainGenerator: No terrain rules defined!");

        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                var worldX = chunk.ChunkX * Chunk.ChunkSize + x;
                var worldY = chunk.ChunkY * Chunk.ChunkSize + y;

                float noiseValue = _perlin.SampleOctaves(worldX * 0.05f, worldY * 0.05f, 4, 0.5f, 2f) * 100f;

                var newTile = SelectTile(noiseValue);

                chunk.Tiles[x, y] = newTile;
            }
        }
    }

    private TileId SelectTile(float value)
    {
        foreach (TerrainRule rule in _rules)
        {
            if (value >= rule.Min && (value < rule.Max || rule.Max == 100))
                return rule.Tile;
        }

        return TileId.Invalid;
    }
}