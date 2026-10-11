using MiniEngine.Components;
using MiniEngine.Entities;

namespace MiniEngine.Environment;

public interface IWorld
{
    public ComponentManager Components { get; }

    public Entity CreateEntity();
    public void DestroyEntity(Entity entity);
}