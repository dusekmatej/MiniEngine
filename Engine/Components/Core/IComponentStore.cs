using MiniEngine.Entities;

namespace MiniEngine.Components.Core;

internal interface IComponentStore<T> where T : struct
{
    int Count { get; }

    bool Has(Entity entity);
    void Add(Entity entity, T component);
    ref T Get(Entity entity);
    bool Remove(Entity entity);
}