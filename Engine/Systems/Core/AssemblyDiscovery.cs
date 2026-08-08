using System.Reflection;

namespace MiniEngine.Systems.Core;

internal static class AssemblyDiscovery
{
    public static IEnumerable<Assembly> Discover()
    {
        var assemblies = new Dictionary<string, Assembly>();

        AddLoadedAssemblies(assemblies);
        AddReferencedAssemblies(assemblies);

        return assemblies.Values;
    }

    private static void AddLoadedAssemblies(Dictionary<string, Assembly> assemblies)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            AddAssembly(assemblies, assembly);
    }

    private static void AddReferencedAssemblies(Dictionary<string, Assembly> assemblies)
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null) return;

        AddAssembly(assemblies, entryAssembly);

        foreach (var reference in entryAssembly.GetReferencedAssemblies())
        {
            try
            {
                AddAssembly(assemblies, Assembly.Load(reference));                            
            }
            catch
            {
                // Ignore references that cannot be loaded
            }
        }
    }

    private static void AddAssembly(Dictionary<string, Assembly> assemblies, Assembly assembly)
    {
        var assemblyName = assembly.FullName;

        if (assemblyName is null) return;

        assemblies.TryAdd(assemblyName, assembly);
    }
}