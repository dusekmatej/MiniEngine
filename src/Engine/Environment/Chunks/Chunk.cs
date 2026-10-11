namespace MiniEngine.Environment;

public class Chunk
{
    public const int ChunkSize = 16;
    public int ChunkX { get; }
    public int ChunkY { get; }

    public Tile[,] Tiles { get; } = new Tile[ChunkSize, ChunkSize];

    public Chunk (int chunkX, int chunkY)
    {
        ChunkX = chunkX;
        ChunkY = chunkY;
    }
}