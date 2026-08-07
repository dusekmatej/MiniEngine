namespace MiniEngine.Engine.Core;

public class Time
{
    public float DeltaTime { get; private set; }
    private DateTime _lastTime;

    public Time()
    {
        _lastTime = DateTime.Now;
    }

    public void Update()
    {
        var currentTime = DateTime.Now;
        DeltaTime = (float)(currentTime - _lastTime).TotalSeconds;
        
        _lastTime = currentTime;
    }
}