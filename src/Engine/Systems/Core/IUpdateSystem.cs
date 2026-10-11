namespace MiniEngine.Systems.Core;

public interface IUpdateSystem : ISystem
{
    void Update(SystemContext x);
}