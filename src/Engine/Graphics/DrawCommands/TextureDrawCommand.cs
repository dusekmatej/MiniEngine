using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct TextureDrawCommand(
    BackendTextureHandle Texture,
    float X,
    float Y,
    float Width,
    float Height,
    float Rotation = 0f,
    Vector2 Scale = default,
    EngineColor Tint = default,
    int Layer = 0)
{
    public Matrix4x4? Transform { get; init; }
    public Matrix4x4? TextureTransform { get; init; }
    public Vector2 Scale { get; init; } = Scale == default ? Vector2.One : Scale;
    public EngineColor Tint { get; init; } = Tint == default ? EngineColor.White : Tint;
}