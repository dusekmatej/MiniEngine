namespace MiniEngine.Entities;

public class EntityManager
{
    private uint _nextId = 0;
    private readonly List<(uint Id, uint Generation)> _entities = new();

    // Creates a new entity gets entities from the list if available and creates with the old id 
    // (from the back of the list) and then takes its generation and increments it by 1. 
    // If no entities are available, it creates a new entity with the next id and generation 0.
    public Entity CreateEntity()
    {
        if (_entities.Count > 0)
        {
            int lastIndex = _entities.Count - 1;
            var (id, generation) = _entities[lastIndex];
            _entities.RemoveAt(lastIndex);


            return new Entity(id, generation + 1);
        }

        return new Entity(_nextId++, 0);
    }

    public void DestroyEntity(Entity entity)
    {
        if (entity.Id >= _nextId)
            throw new ArgumentOutOfRangeException("Entity ID is out of range.");

        _entities.Add((entity.Id, entity.Generation));
    }
}