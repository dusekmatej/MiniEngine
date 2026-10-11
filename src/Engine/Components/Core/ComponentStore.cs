using MiniEngine.Entities;

namespace MiniEngine.Components.Core;

internal sealed class ComponentStore<T> where T : struct
{
    private T[] _components;
    private uint[] _entities;
    private int[] _sparse;
    private int _count;

    public int Count => _count;

    public ComponentStore(int capacity = 16)
    {
        _components = new T[capacity];
        _entities = new uint[capacity];
        _sparse = new int[capacity];

        Array.Fill(_sparse, -1);
    }

    public bool Has(Entity entity)
    {
        uint id = entity.Id;

        if ((uint)id >= (uint)_sparse.Length)
            return false;

        int denseIndex = _sparse[id];

        return denseIndex >= 0
            && denseIndex < _count
            && _entities[denseIndex] == id;
    }

    public void Add(Entity entity, T component)
    {
        if (entity.Id >= (uint)Array.MaxLength)
            throw new ArgumentOutOfRangeException(nameof(entity), "Entity ID exceeds supported storage capacity.");

        EnsureSparseCapacity((int)entity.Id);

        if (Has(entity))
        {
            _components[_sparse[entity.Id]] = component;
            return;
        }

        EnsureDenseCapacity();

        int denseIndex = _count++;

        _components[denseIndex] = component;
        _entities[denseIndex] = entity.Id;
        _sparse[entity.Id] = denseIndex;
    }

    public ref T Get(Entity entity)
    {
        if (!Has(entity))
            throw new InvalidOperationException(
                $"Entity {entity.Id} does not have component {typeof(T).Name}.");

        return ref _components[_sparse[entity.Id]];
    }

    public bool Remove(Entity entity)
    {
        if (!Has(entity))
            return false;

        int removedIndex = _sparse[entity.Id];
        int lastIndex = _count - 1;

        if (removedIndex != lastIndex)
        {
            _components[removedIndex] = _components[lastIndex];

            uint movedEntity = _entities[lastIndex];
            _entities[removedIndex] = movedEntity;
            _sparse[movedEntity] = removedIndex;
        }

        _components[lastIndex] = default;
        _entities[lastIndex] = 0;
        _sparse[entity.Id] = -1;

        _count--;
        return true;
    }

    private void EnsureDenseCapacity()
    {
        if (_count < _components.Length)
            return;

        int newCapacity = _components.Length * 2;

        Array.Resize(ref _components, newCapacity);
        Array.Resize(ref _entities, newCapacity);
    }

    private void EnsureSparseCapacity(int entityId)
    {
        if (entityId < _sparse.Length)
            return;

        int oldLength = _sparse.Length;
        int newLength = Math.Max(entityId + 1, oldLength * 2);

        Array.Resize(ref _sparse, newLength);
        Array.Fill(_sparse, -1, oldLength, newLength - oldLength);
    }
}