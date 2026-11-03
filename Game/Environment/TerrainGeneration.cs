using MiniEngine.Environment;

namespace MiniEngine.Game.Environment;

public sealed class TerrainGeneration
{
    private readonly TerrainGenerator _generator;
    private readonly Perlin _grassCoverage = new(91234);
    private readonly Dictionary<TileId, TileId[]> _variations = new();
    private readonly TileId _grass;

    public TileDefinitionRegistry Definitions { get; } = new();
    public TerrainGeneration()
    {
        TileId deepWater = Register("Water", 092);
        TileId water = Register("Water", 104);
        TileId shore = Register("Dirt", 000, 001, 002, 008, 009, 010);
        _grass = Register("Grass", 028, 027, 035);
        TileId earth = Register("Dirt", 003);
        TileId stone = Register("Rocks", 063);

        _generator = new TerrainGenerator(12345, Definitions);
        // Octave noise clusters around 50, so useful biome bands belong near the middle.
        _generator.Add(Definitions.Get(deepWater).Key, 0, 39);
        _generator.Add(Definitions.Get(water).Key, 39, 44);
        _generator.Add(Definitions.Get(shore).Key, 44, 47);
        _generator.Add(Definitions.Get(_grass).Key, 47, 59);
        _generator.Add(Definitions.Get(earth).Key, 59, 63);
        _generator.Add(Definitions.Get(stone).Key, 63, 100);
    }

    public Chunk Generate(int chunkX, int chunkY)
    {
        var chunk = new Chunk(chunkX, chunkY);
        _generator.Generate(chunk);

        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                int worldX = chunkX * Chunk.ChunkSize + x;
                int worldY = chunkY * Chunk.ChunkSize + y;
                uint variation = CoordinateHash(worldX, worldY);
                TileId ground = chunk.Tiles[x, y];
                TileId[] variants = _variations[ground];
                int variantIndex = (int)(variation % (uint)variants.Length);
                if (ground == _grass)
                {
                    // Similar green shades form broad patches instead of isolated bright squares.
                    float coverage = _grassCoverage.Sample(worldX * 0.08f, worldY * 0.08f);
                    variantIndex = 0;
                    if (coverage < 0.43f)
                    {
                        variantIndex = 1;
                    }
                    else if (coverage > 0.57f)
                    {
                        variantIndex = 2;
                    }
                }
                chunk.Tiles[x, y] = variants[variantIndex];
            }
        }

        return chunk;
    }

    private TileId Register(string folder, params int[] tiles)
    {
        var ids = new TileId[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            string texture = $"Terrain/{folder}/tile_{tiles[i]:000}";
            ids[i] = Definitions.Register(new TileDefinition(texture, texture));
        }
        _variations.Add(ids[0], ids);
        return ids[0];
    }

    private static uint CoordinateHash(int x, int y)
    {
        // Stable at negative coordinates too; revisiting an unloaded chunk restores it exactly.
        unchecked
        {
            uint hash = (uint)x * 374761393u + (uint)y * 668265263u + 12345u;
            hash = (hash ^ (hash >> 13)) * 1274126177u;
            return hash ^ (hash >> 16);
        }
    }
}
