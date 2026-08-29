using MiniEngine.Engine.Components.Core;

namespace MiniEngine.Engine.Components;

public struct MovementConstrainComponent : IComponent
{
    public bool ConstrainX;
    public bool ConstrainY;
}