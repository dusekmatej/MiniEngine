namespace MiniEngine.Environment;

public class Scene : IScene
{
    private readonly List<IWorld> _worlds = new List<IWorld>();
    public IReadOnlyList<IWorld> Worlds => _worlds.AsReadOnly(); // So we cannot modify the worlds from outside the defined scope of editing

    public Scene()
    {
    }

    public void AddWorld(IWorld world)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (_worlds.Contains(world))
            throw new InvalidOperationException("World is already added to the scene.");

        _worlds.Add(world);
    }

    public bool RemoveWorld(IWorld world)
        => _worlds.Remove(world);
}