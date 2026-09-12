using MiniEngine.Entities;

namespace MiniEngine.Components.Core;

public sealed class ComponentManager
{
    private readonly Dictionary<Type, IComponentStorage> _storages = [];

    


    // Get or create storage
    private ComponentStorage<T> GetStorage<T>() where T : struct, IComponent
    {
        Type type = typeof(T);

        if (_storages.TryGetValue(type, out IComponentStorage? existingStorage))
            return (ComponentStorage<T>)existingStorage;

        return CreateStorage<T>();
    }    

    private ComponentStorage<T> CreateStorage<T>() where T : struct, IComponent
    {
        var newStorage = new ComponentStorage<T>();
        _storages[typeof(T)] = newStorage;

        return newStorage;
    }
}