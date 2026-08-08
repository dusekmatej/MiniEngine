namespace MiniEngine.Systems.Core;

public readonly struct SystemContext
{
    public float DeltaTime { get; }
    internal SystemContext(float deltaTime)
    {
        DeltaTime = deltaTime;
    }
}