using MiniEngine.Environment;
namespace MiniEngine.Game.Environment;

public class MainWorld : World
{
    // Create the blank map
    public const int GridSize = 9;
    public TileMap Map { get; } = new(GridSize, GridSize);

    private TileDefinitionRegistry _tdm = new();

    public MainWorld()
    {
        var grass = _tdm.Register(new TileDefinition("Grass", "Terrain/Grass/tile_040"));
        var water = _tdm.Register(new TileDefinition("Water", "Terrain/Water/tile_109"));

    }   
}