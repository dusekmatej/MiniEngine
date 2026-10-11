namespace MiniEngine.Systems.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PriorityAttribute : Attribute
{
    public int Value { get; }

    public PriorityAttribute(PriorityLevel level)
        => Value = (int)level;
    
    public PriorityAttribute(int value)
        => Value = value;
}