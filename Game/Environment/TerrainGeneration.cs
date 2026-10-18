using MiniEngine.Environment;

namespace MiniEngine.Game.Environment;

public sealed class TerrainGeneration
{
    private readonly TerrainGenerator _generator;
    private readonly Perlin _vegetation = new(54321);
    private readonly Dictionary<TileId, TileId[]> _variations = new();
    private readonly TileId _grass;
    private readonly TileId _water;
    private readonly TileId _stone;
    private readonly TileId[] _flowers;
    private readonly TileId[] _rocks;
    private readonly TileId[] _waterRocks;

    public TileDefinitionRegistry Definitions { get; } = new();

    public TerrainGeneration()
    {
        TileId deepWater = Register("Water", 092, 093, 094);
        _water = Register("Water", 109, 104, 105, 106, 107, 108, 110, 111, 112, 113);
        TileId shore = Register("Dirt", 000, 001, 002, 008, 009, 010);
        _grass = Register("Grass", 040, 037, 038, 039, 022, 024);
        TileId earth = Register("Dirt", 017, 018, 021);
        _stone = Register("Rocks", 063);
        _flowers = RegisterDecorations("Flowers", 041, 042, 044, 046, 047);
        _rocks = RegisterDecorations("Rocks", 064, 065, 067, 068);
        _waterRocks = RegisterDecorations("WaterRocks", 071, 072, 073, 074, 075, 076);

        _generator = new TerrainGenerator(12345, Definitions);
        // Octave noise clusters around 50, so useful biome bands belong near the middle.
        _generator.Add(Definitions.Get(deepWater).Key, 0, 39);
        _generator.Add(Definitions.Get(_water).Key, 39, 44);
        _generator.Add(Definitions.Get(shore).Key, 44, 47);
        _generator.Add(Definitions.Get(_grass).Key, 47, 59);
        _generator.Add(Definitions.Get(earth).Key, 59, 63);
        _generator.Add(Definitions.Get(_stone).Key, 63, 100);
    }

    public TerrainChunk Generate(int chunkX, int chunkY)
    {
        var terrain = new TerrainChunk(new Chunk(chunkX, chunkY));
        _generator.Generate(terrain.Ground);

        for (int x = 0; x < Chunk.ChunkSize; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                int worldX = chunkX * Chunk.ChunkSize + x;
                int worldY = chunkY * Chunk.ChunkSize + y;
                uint variation = CoordinateHash(worldX, worldY);
                TileId ground = terrain.Ground.Tiles[x, y];
                TileId[] variants = _variations[ground];
                // Keep most grass plain so the detailed variations form accents.
                int variantIndex = 0;
                if (ground != _grass || variation % 5 == 0)
                {
                    variantIndex = (int)(variation % (uint)variants.Length);
                }
                terrain.Ground.Tiles[x, y] = variants[variantIndex];

                float vegetation = _vegetation.Sample(worldX * 0.09f, worldY * 0.09f);
                uint decoration = CoordinateHash(worldX + 731, worldY - 193);
                if (ground == _grass && vegetation > 0.48f && decoration % 13 == 0)
                {
                    terrain.Decorations[x, y] = _flowers[decoration % (uint)_flowers.Length];
                }
                else if (ground == _stone && decoration % 9 == 0)
                {
                    terrain.Decorations[x, y] = _rocks[decoration % (uint)_rocks.Length];
                }
                else if (ground == _water && decoration % 47 == 0)
                {
                    terrain.Decorations[x, y] = _waterRocks[decoration % (uint)_waterRocks.Length];
                }
            }
        }

        return terrain;
    }

    private TileId Register(string folder, params int[] tiles)
    {
        TileId[] variants = RegisterDecorations(folder, tiles);
        _variations.Add(variants[0], variants);
        return variants[0];
    }

    private TileId[] RegisterDecorations(string folder, params int[] tiles)
    {
        var ids = new TileId[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            string texture = $"Terrain/{folder}/tile_{tiles[i]:000}";
            ids[i] = Definitions.Register(new TileDefinition(texture, texture));
        }
        return ids;
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
