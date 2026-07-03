namespace Engine.Core;

public class Engine
{
    private Window _window; // Don't mistake the IWindow with Window class
    private GameLoop _loop;
    private IGame _game;

    public Engine()
    {
        _game.Initialize();
        _window = new Window();

        var time = new Time();
        _loop = new GameLoop(time);

        _window.Update += _ =>
        {
            Console.WriteLine($"DeltaTime: {time.DeltaTime:F10}");
            _loop.Tick();
            _game.Update(time.DeltaTime);
        };

        _window.Render += _ =>
        {
            _game.Render();
        };
    }

    public void Run()
    {
        _window.Run();
    }
}