namespace MiniEngine.Graphics;

public readonly record struct RectangleDrawCommand(
    float X,
    float Y,
    float Width,
    float Height,
    EngineColor Color
);