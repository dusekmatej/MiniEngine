using MiniEngine.Environment;

namespace MiniEngine.Game.Environment;

public class MainScene : Scene
{
    public MainWorld World;

    public MainScene()
    {
        World = new MainWorld();

        AddWorld(World);
    }
}