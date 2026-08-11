internal sealed class ComponentStore<T> : IComponentStore
    where T : struct, IComponent
{
    private int[] _sparse = [];
    private int[] _denseEntities = [];
    private T[] _denseComponents = [];

    private int _count;
    public int Count => _count;

    public void Add(int entityIndex, T component)
    {
        if (Has(entityIndex))
    throw new InvalidOperationException("Entity already has this component.");

        if (entityIndex >= _sparse.Length)
            resize(entityIndex);

        if (_denseEntities.Length <= _count)
            Array.Resize(ref _denseEntities, Math.Max(4, _denseComponents.Length * 2));

        if (_denseComponents.Length <= _count)
            Array.Resize(ref _denseComponents, Math.Max(4, _denseComponents.Length * 2));

        _denseEntities[_count] = entityIndex;
        _denseComponents[_count] = component;

        _sparse[entityIndex] = _count;

        _count++;
    }

    public bool Has(int entityIndex)
    {
        if (entityIndex < 0 || entityIndex >= _sparse.Length)
            return false;

        return _sparse[entityIndex] < _count && _denseEntities[_sparse[entityIndex]] == entityIndex;
    }

    public ref T Get(int entityIndex)
    {
        if (entityIndex < 0 || entityIndex >= _sparse.Length)
            throw new ArgumentOutOfRangeException(nameof(entityIndex));

        int componentIndex = _sparse[entityIndex];
        if (componentIndex >= _count || _denseEntities[componentIndex] != entityIndex)
            throw new InvalidOperationException("Component not found");

        return ref _denseComponents[componentIndex];
    }

    public bool Remove(int entityIndex)
    {
    if (!Has(entityIndex))
        return false;

    int removeIndex = _sparse[entityIndex];
    int lastIndex = _count - 1;

    if (removeIndex != lastIndex)
    {
        int lastEntityIndex = _denseEntities[lastIndex];

        _denseEntities[removeIndex] = lastEntityIndex;
        _denseComponents[removeIndex] = _denseComponents[lastIndex];

        _sparse[lastEntityIndex] = removeIndex;
    }

    _count--;

    return true;
}
public bool TryGet(int entityIndex, out T component)
{
    if (Has(entityIndex))
    {
        component = component = Get(entityIndex);
        return true;
    }

    component = default;
    return false;
}