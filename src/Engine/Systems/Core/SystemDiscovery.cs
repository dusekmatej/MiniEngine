using System.Reflection;

namespace MiniEngine.Systems.Core;

internal static class SystemDiscovery
{
    
    // Discover all systems
    public static List<SystemInfo> Discover()
    {
        var systemInfos = new List<SystemInfo>();

        foreach (var assembly in AssemblyDiscovery.Discover())
        {
            foreach (var type in GetLoadableTypes(assembly))
            {
                if (!IsSystem(type)) continue;

                systemInfos.Add(SystemFactory.Create(type));
            }
        }

        return systemInfos;
    }

    private static bool IsSystem(Type type)
    {
        return type.IsClass 
            && !type.IsAbstract 
            && typeof(ISystem).IsAssignableFrom(type);        
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types
                    .Where(t => t != null)
                    .Cast<Type>();
        }
    }
}