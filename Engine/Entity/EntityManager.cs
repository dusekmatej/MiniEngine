namespace MiniEngine.Entity;

public class EntityManager
{
    private readonly Stack<int> _freeIds = new();
    private readonly List<uint> _generations = new();

    public Entity Create()
    {
        if (_freeIds.Count > 0)
        {
            int id = _freeIds.Pop();
            return new Entity(id, _generations[id]);
        }

        int newId = _generations.Count;

        _generations.Add(1);

        return new Entity(newId, 1);
    }

    public void Destroy(Entity other)
    {
        if (!IsAlive(other))
            return;

        _generations[other.Id]++;
        _freeIds.Push(other.Id);
    }

    public bool IsAlive(Entity entity)
    {
        if ((uint)entity.Id >= (uint)_generations.Count) return false;

        return _generations[entity.Id] == entity.Generation;
    }
}