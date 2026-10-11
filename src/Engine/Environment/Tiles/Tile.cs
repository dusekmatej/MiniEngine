namespace MiniEngine.Environment;

public class Tile
{
    public TileId DefinitionId { get; set; }
    public byte Variation { get; set; }

    public Tile(TileId definitionId, byte variation = 0)
    {
        DefinitionId = definitionId;
        Variation = variation;
    }
}