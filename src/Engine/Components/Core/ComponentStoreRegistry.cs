namespace MiniEngine.Components.Core;

internal sealed class ComponentStoreRegistry
{
    private readonly Dictionary<Type, object> _stores = new();

    public ComponentStore<T> GetOrCreate<T>() where T : struct
    {
        var componentType = typeof(T);

        if (_stores.TryGetValue(componentType, out var store))
            return (ComponentStore<T>)store;

        var newStore = new ComponentStore<T>();
        _stores.Add(componentType, newStore);

        return newStore;
    }

    public ComponentStore<T>? Get<T>() where T : struct
    {
        var componentType = typeof(T);

        if (_stores.TryGetValue(componentType, out var store))
            return (ComponentStore<T>)store;

        return null;
    }
}
