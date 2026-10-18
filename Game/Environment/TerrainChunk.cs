using MiniEngine.Environment;

namespace MiniEngine.Game.Environment;

public sealed class TerrainChunk(Chunk ground)
{
    public Chunk Ground { get; } = ground;
    public TileId[,] Decorations { get; } = new TileId[Chunk.ChunkSize, Chunk.ChunkSize];
}
