using MiniEngine.Entities;

namespace MiniEngine.Components.Core;

public class Components
{
    private ComponentStoreRegistry _registry;

    internal Components(ComponentStoreRegistry componentStores)
    {
        _registry = componentStores;
    }

    public void Add<T>(Entity entity, T component) where T : struct
        => _registry.GetOrCreate<T>().Add(entity, component); // We call the registry to get specific store then we add to the store

    public bool Has<T>(Entity entity) where T : struct
        => _registry.Get<T>()?.Has(entity) ?? false; // We call the registry to get specific store then we check if the store has the entity

    public void Remove<T>(Entity entity) where T : struct
        => _registry.Get<T>()?.Remove(entity); // We call the registry to get specific store then we remove from the store
}