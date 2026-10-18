using MiniEngine.Environment;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Presets;

namespace MiniEngine.Game.Environment;

public class MainWorld : World
{
    // Kept for the separate platform drawing demo.
    public const int GridSize = 9;
    public TileMap Map { get; } = new(GridSize, GridSize);

    private readonly TerrainGeneration _terrain = new();
    private readonly Dictionary<(int X, int Y), TerrainChunk> _chunks = new();
    private readonly List<(int X, int Y)> _unloaded = new();

    public TileDefinitionRegistry TileDefinitions => _terrain.Definitions;
    public int LoadedChunkCount => _chunks.Count;
    public int GeneratedChunkCount { get; private set; }
    public int UnloadedChunkCount { get; private set; }
    public int MinimumTileX { get; private set; }
    public int MaximumTileX { get; private set; }
    public int MinimumTileY { get; private set; }
    public int MaximumTileY { get; private set; }

    public void UpdateVisibleChunks(Camera camera, IsometricPreset preset, float aspectRatio)
    {
        // Include the entire sprite at screen edges and a small streaming margin.
        float padding = preset.SpriteHeight + 0.5f;
        float left = camera.Position.X - (aspectRatio + padding) / camera.Zoom;
        float right = camera.Position.X + (aspectRatio + padding) / camera.Zoom;
        float bottom = camera.Position.Y - (1f + padding) / camera.Zoom;
        float top = camera.Position.Y + (1f + padding) / camera.Zoom;

        // Inverse of the isometric footprint projection, including negative coordinates.
        MinimumTileX = (int)MathF.Floor(left / preset.FootprintWidth - top / preset.FootprintHeight);
        MaximumTileX = (int)MathF.Ceiling(right / preset.FootprintWidth - bottom / preset.FootprintHeight);
        MinimumTileY = (int)MathF.Floor(-right / preset.FootprintWidth - top / preset.FootprintHeight);
        MaximumTileY = (int)MathF.Ceiling(-left / preset.FootprintWidth - bottom / preset.FootprintHeight);

        int minimumChunkX = (int)MathF.Floor((float)MinimumTileX / Chunk.ChunkSize);
        int maximumChunkX = (int)MathF.Floor((float)MaximumTileX / Chunk.ChunkSize);
        int minimumChunkY = (int)MathF.Floor((float)MinimumTileY / Chunk.ChunkSize);
        int maximumChunkY = (int)MathF.Floor((float)MaximumTileY / Chunk.ChunkSize);

        _unloaded.Clear();
        foreach (var entry in _chunks)
        {
            if (!IntersectsView(entry.Key.X, entry.Key.Y, preset, left, right, bottom, top))
            {
                _unloaded.Add(entry.Key);
            }
        }
        foreach (var coordinate in _unloaded)
        {
            _chunks.Remove(coordinate);
            UnloadedChunkCount++;
        }

        for (int x = minimumChunkX; x <= maximumChunkX; x++)
        {
            for (int y = minimumChunkY; y <= maximumChunkY; y++)
            {
                if (!_chunks.ContainsKey((x, y)) && IntersectsView(x, y, preset, left, right, bottom, top))
                {
                    _chunks.Add((x, y), _terrain.Generate(x, y));
                    GeneratedChunkCount++;
                }
            }
        }
    }

    public bool TryGetTile(int x, int y, out TileId ground, out TileId decoration)
    {
        int chunkX = (int)MathF.Floor((float)x / Chunk.ChunkSize);
        int chunkY = (int)MathF.Floor((float)y / Chunk.ChunkSize);
        if (_chunks.TryGetValue((chunkX, chunkY), out TerrainChunk? chunk))
        {
            int localX = x - chunkX * Chunk.ChunkSize;
            int localY = y - chunkY * Chunk.ChunkSize;
            ground = chunk.Ground.Tiles[localX, localY];
            decoration = chunk.Decorations[localX, localY];
            return true;
        }

        ground = TileId.Invalid;
        decoration = TileId.Invalid;
        return false;
    }

    private static bool IntersectsView(int chunkX, int chunkY, IsometricPreset preset,
        float left, float right, float bottom, float top)
    {
        int x = chunkX * Chunk.ChunkSize;
        int y = chunkY * Chunk.ChunkSize;
        float minimumX = (x - y - Chunk.ChunkSize + 1) * preset.FootprintWidth / 2f;
        float maximumX = (x - y + Chunk.ChunkSize - 1) * preset.FootprintWidth / 2f + preset.SpriteWidth;
        float minimumY = -(x + y + 2 * (Chunk.ChunkSize - 1)) * preset.FootprintHeight / 2f;
        float maximumY = -(x + y) * preset.FootprintHeight / 2f + preset.SpriteHeight;
        return maximumX >= left && minimumX <= right && maximumY >= bottom && minimumY <= top;
    }
}
