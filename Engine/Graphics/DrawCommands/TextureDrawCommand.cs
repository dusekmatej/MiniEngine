namespace MiniEngine.Graphics;

public readonly record struct TextureDrawCommand(
    BackendTextureHandle Texture,
    float X,
    float Y,
    float Width,
    float Height
) {};