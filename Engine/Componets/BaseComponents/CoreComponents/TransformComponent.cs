using System.Numerics;
using MiniEngine.Components.Core;

namespace MiniEngine.Components;

public struct TransformComponent : IComponent
{
    public float X;
    public float Y;
    public Vector2 Scale;
    public float Rotation;
}
    
