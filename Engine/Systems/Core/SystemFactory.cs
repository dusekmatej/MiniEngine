using System.Reflection;

namespace MiniEngine.Systems.Core;

internal static class SystemFactory
{
    public static SystemInfo Create(Type type)
    {
        var system = CreateSystem(type);
        var priority = GetPriority(type);

        return new SystemInfo(system, priority);
    }

    private static ISystem CreateSystem(Type type)
    {
        if (Activator.CreateInstance(type) is ISystem system)
            return system;
        
        throw new InvalidOperationException($"Could not create an instance of {type.FullName}");
    }

    private static int GetPriority(Type type)
    {
        return type.GetCustomAttribute<PriorityAttribute>()?.Value
            ?? (int)PriorityLevel.Normal;
    } 
}