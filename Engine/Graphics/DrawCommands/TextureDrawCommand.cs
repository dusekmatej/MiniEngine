using System.Numerics;

namespace MiniEngine.Graphics;

public readonly record struct TextureDrawCommand
{
    public BackendTextureHandle Texture { get; }
    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }
    public float Rotation { get; }
    public Vector2 Scale { get; }
    public EngineColor Tint { get; }
    public int Layer { get; }

    public TextureDrawCommand(
        BackendTextureHandle texture,
        float x,
        float y,
        float width,
        float height)
        : this(
            texture,
            x,
            y,
            width,
            height,
            0f,
            Vector2.One,
            EngineColor.White,
            0)
    {
    }

    public TextureDrawCommand(
        BackendTextureHandle texture,
        float x,
        float y,
        float width,
        float height,
        float rotation,
        Vector2 scale,
        EngineColor tint,
        int layer)
    {
        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Rotation = rotation;
        Scale = scale;
        Tint = tint;
        Layer = layer;
    }
}