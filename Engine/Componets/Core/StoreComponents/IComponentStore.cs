namespace MiniEngine.Components.Core.StoreComponents;
internal interface IComponentStore
{
    int Count { get; }
    bool Has(int EntityIndex);
    bool Remove(int EntityIndex);
} 