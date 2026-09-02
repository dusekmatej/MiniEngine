namespace MiniEngine.Graphics;

public readonly record struct TextDrawCommand(
    BackendTextureHandle Texture,
    float X,
    float Y,
    float Width,
    float Height,
    EngineColor Color
);