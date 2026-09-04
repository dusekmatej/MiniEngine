namespace MiniEngine.Components.Core;
internal interface IComponentStorage
{
    int Count { get; }
    bool Has(int EntityIndex);
    bool Remove(int EntityIndex);
} 