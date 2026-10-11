namespace MiniEngine.Environment;

public interface IScene
{
    IReadOnlyList<IWorld> Worlds { get; }

    void AddWorld(IWorld world);
    bool RemoveWorld(IWorld world);
}