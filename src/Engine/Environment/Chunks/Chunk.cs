namespace MiniEngine.Environment;

public class Chunk
{
    public const int ChunkSize = 16;
    public int ChunkX { get; }
    public int ChunkY { get; }

    public TileId[,] Tiles { get; } = new TileId[ChunkSize, ChunkSize];

    public Chunk (int chunkX, int chunkY)
    {
        ChunkX = chunkX;
        ChunkY = chunkY;
    }
}