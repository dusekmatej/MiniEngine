using System.Numerics;
using MiniEngine.Engine.Components.Core;

namespace MiniEngine.Engine.Components;

public struct WorldTransformComponent : IComponent
{
    public float Rotation;
    public Vector2 Scale;
    public Vector3 Position;
}