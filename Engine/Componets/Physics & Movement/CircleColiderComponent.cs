using MiniEngine.Engine.Components.Core;

namespace MiniEngine.Engine.Components;

public struct CircleColliderComponent : IComponent
{
    public float Radius;
    public float OffsetX;
    public float OffsetY;
}