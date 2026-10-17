using MiniEngine.Environment;

namespace MiniEngine.Game.Environment;

public class MainScene : Scene
{
    public MainWorld MainWorld;

    public MainScene()
    {
        MainWorld = new MainWorld();
    
        AddWorld(MainWorld);
    }
}