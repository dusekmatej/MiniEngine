namespace MiniEngine.Graphics;

public sealed class TileMap
{
    private readonly int[] _tiles;

    public int Width { get; } 
    public int Height { get; }
    public int Count => _tiles.Length;

    public TileMap(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException("Map width must be higher than zero.");

       if (height <= 0)
            throw new ArgumentOutOfRangeException("Map height must be higher than zero.");

        Width = width;
        Height = height;

        _tiles = new int[width * height]; 
    }

    public int this[int x, int y]
    {
        get => _tiles[GetIndex(x, y)];
        set => _tiles[GetIndex(x, y)] = value;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width 
            && y >= 0 && y < Height;
    }

    private int GetIndex(int x, int y)
    {
        if (!IsInBounds(x, y))
            throw new ArgumentOutOfRangeException($"Tile position ({x}, {y}) is out of bounds." + $"Size of map {Width}x{Height}");

        return y * Width + x;
    }
}