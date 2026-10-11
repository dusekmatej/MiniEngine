using MiniEngine.Components;
using MiniEngine.Entities;

namespace MiniEngine.Environment;

public class World : IWorld
{
    private readonly EntityManager _entityManager;
    private readonly ComponentStoreRegistry _componentRegistry;
    public ComponentManager Components { get; }

    public World()
    {
        _componentRegistry = new ComponentStoreRegistry();
        Components = new ComponentManager(_componentRegistry);
    }

    public Entity CreateEntity()
        => _entityManager.CreateEntity();

    public void DestroyEntity(Entity entity) 
        => _entityManager.DestroyEntity(entity);
}