using MiniEngine.Components;
using MiniEngine.Entities;

namespace MiniEngine.Environment;

public sealed class World : IWorld
{
    private readonly ComponentStoreRegistry _componentRegistry;
    private readonly EntityManager _entityManager;
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