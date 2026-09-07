using MiniEngine.Entity;

namespace MiniEngine.Environment;

public sealed class World : IWorld
{
    private EntityManager _entityManager;

    public World(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }
}