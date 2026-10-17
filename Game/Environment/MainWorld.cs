using MiniEngine.Environment;
namespace MiniEngine.Game.Environment;

public class MainWorld : World
{
    // Create the blank map
    public const int GridSize = 9;
    public TileMap Map { get; } = new(GridSize, GridSize);

    public TileDefinitionRegistry TileDefinitions = new();

    public Chunk Chunk { get; private set;}

    public MainWorld()
    {
        TestWorld();
    }   

    public void TestWorld()
    {
        TileDefinitions.Register(new TileDefinition("Grass", "Terrain/Grass/tile_040"));
        TileDefinitions.Register(new TileDefinition("Water", "Terrain/Water/tile_109"));

        var generator = new TerrainGenerator(12345, TileDefinitions);
        generator.Add("Water", 0, 45);
        generator.Add("Grass", 45, 100);

        Chunk = new Chunk(0, 0);
        generator.Generate(Chunk);
    }
}