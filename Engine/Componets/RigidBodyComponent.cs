using MiniEngine.Engine.Components.Core;

namespace MiniEngine.Engine.Components;

public struct RigidBodyComponent : IComponent
{
    public float Mass;
    public bool UseGravity;
    public bool IsStatic;
}