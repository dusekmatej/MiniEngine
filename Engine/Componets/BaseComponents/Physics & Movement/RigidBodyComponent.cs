using MiniEngine.Components.Core;

namespace MiniEngine.Components;

public struct RigidBodyComponent : IComponent
{
    public float Mass;
    public bool UseGravity;
    public bool IsStatic;
}