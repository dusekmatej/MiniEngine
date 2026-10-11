namespace MiniEngine.Systems.Core;

internal sealed class SystemInfo
{
    public ISystem System { get; }
    public int Priority { get; }

    public SystemInfo(ISystem system, int priority)
    {
        System = system;
        Priority = priority;
    }
}