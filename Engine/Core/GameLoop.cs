namespace Engine.Core;

public class GameLoop
{
    private readonly Time _time;

    public event Action<float>? Update;
    public event Action? Render;
    
    public GameLoop(Time time)
        => _time = time;

    public void Tick()
    {
        _time.Update();

        // Invoke the main Update & Render events
        Update?.Invoke(_time.DeltaTime);
        Render?.Invoke();
    }
}