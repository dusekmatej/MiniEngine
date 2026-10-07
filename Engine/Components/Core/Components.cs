namespace MiniEngine.Components.Core;

internal class Components
{
    public ComponentStoreRegistry ComponentStoreRegistry;

    public Components(ComponentStoreRegistry componentStores)
    {
        ComponentStoreRegistry = componentStores;
    }

    public bool Add<T>() where T : struct
    {
        var store = ComponentStoreRegistry.GetOrCreate<T>();
    }
}