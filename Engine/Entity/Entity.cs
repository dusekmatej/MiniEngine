namespace MiniEngine.Entities;

public readonly record struct Entity
{
    public readonly int Id;
    public readonly uint Generation;

    internal Entity(int id, uint version)
    {
        Id = id;
        Generation = version;
    }

    public bool Equals(Entity other)
        => Id == other.Id && Generation == other.Generation;

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Generation);
    }
}