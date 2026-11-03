using MiniEngine.Environment;

namespace MiniEngine.Game.Environment;

public sealed class TerrainGeneration
{
    public const int MaximumHeight = 18;
    public const int PlainsHeight = 1;
    public const float MountainThreshold = 0.60f;

    private readonly TerrainGenerator _generator;
    private readonly Perlin _grassCoverage = new(91234);
    private readonly Perlin _mountainRegions = new(24680);
    private readonly Perlin _mountainDetail = new(67890);
    private readonly Dictionary<TileId, TileId[]> _variations = new();
    private readonly TileId _grass;
    private readonly TileId _earth;
    private readonly TileId _stone;

    public TileDefinitionRegistry Definitions { get; } = new();
    public TerrainGeneration()
    {
        TileId deepWater = Register("Water", "tile_092");
        TileId water = Register("Water", "tile_104");
        TileId shore = Register("isometric_assets", "dirt1");
        _grass = Register("isometric_assets", "grass1", "grass2", "grass7");
        _earth = Register("isometric_assets", "dirt4");
        _stone = Register("isometric_assets", "stone1", "stone2");

        _generator = new TerrainGenerator(12345, Definitions);
        _generator.Add(Definitions.Get(deepWater).Key, 0, 34);
        _generator.Add(Definitions.Get(water).Key, 34, 40);
        _generator.Add(Definitions.Get(shore).Key, 40, 43);
        _generator.Add(Definitions.Get(_grass).Key, 43, 100);
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
                int height = GetHeight(worldX, worldY);
                TileId ground = chunk.Tiles[x, y];
                if (height >= 9)
                {
                    ground = _stone;
                }
                else if (height >= 6)
                {
                    ground = _earth;
                }
                else if (height > 0)
                {
                    ground = _grass;
                }
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
                chunk.Heights[x, y] = height;
            }
        }

        return chunk;
    }

    public int GetHeight(int worldX, int worldY)
    {
        int plainsHeight = 0;
        if (_generator.SampleNoise(worldX, worldY) >= 43f)
        {
            plainsHeight = PlainsHeight;
        }

        // A separate low-frequency field reserves most land for uninterrupted plains.
        float region = _mountainRegions.SampleOctaves(
            worldX * 0.008f + 17f, worldY * 0.008f - 9f, 2, 0.45f, 2f);
        if (region <= MountainThreshold)
        {
            return plainsHeight;
        }

        float influence = Math.Clamp((region - MountainThreshold) / 0.18f, 0f, 1f);
        influence = influence * influence * (3f - 2f * influence);
        float detail = _mountainDetail.SampleOctaves(worldX * 0.025f, worldY * 0.025f, 3, 0.4f, 2f);
        float ridge = 1f - MathF.Abs(detail * 2f - 1f);
        int mountainHeight = (int)MathF.Round(influence * MaximumHeight * (0.6f + ridge * 0.4f));
        return Math.Max(plainsHeight, mountainHeight);
    }

    private TileId Register(string folder, params string[] tiles)
    {
        var ids = new TileId[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            string texture = $"Terrain/{folder}/{tiles[i]}";
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
