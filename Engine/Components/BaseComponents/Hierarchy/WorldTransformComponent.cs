using System.Numerics;
using MiniEngine.Components.Core;

namespace MiniEngine.Components;

public struct WorldTransformComponent : IComponent
{
    public float Rotation;
    public Vector2 Scale;
    public Vector3 Position;
}