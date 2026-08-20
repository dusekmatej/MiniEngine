using MiniEngine.Engine.Components.Core;

namespace MiniEngine.Engine.Components;

public struct BoxColliderComponent : IComponent
{
    public float Width;
    public float Height;
    public float OffsetX;
    public float OffsetY;
}