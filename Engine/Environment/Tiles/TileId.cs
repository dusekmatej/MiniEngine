namespace MiniEngine.Environment;

public readonly record struct TileId(ushort Value)
{
    public static readonly TileId Invalid = new(0);
    public bool IsValid => Value != 0;
}