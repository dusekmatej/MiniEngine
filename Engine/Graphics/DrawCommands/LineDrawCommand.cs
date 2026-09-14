namespace MiniEngine.Graphics;

public readonly record struct LineDrawCommand(
    float StartX,
    float StartY,
    float EndX,
    float EndY,
    float Thickness,
    EngineColor Color,
    int Layer = 0);