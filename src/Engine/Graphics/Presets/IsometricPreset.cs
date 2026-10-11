namespace MiniEngine.Graphics.Presets;

public readonly record struct IsometricPreset(
    float SpriteWidth,
    float SpriteHeight,
    float FootprintWidth,
    float FootprintHeight,
    float ElevationStep
)
{
    public static IsometricPreset Default { get; } = new(
        SpriteWidth: 0.30f,
        SpriteHeight: 0.30f,
        FootprintWidth: 0.30f,
        FootprintHeight: 0.15f,
        ElevationStep: 0.075f
    );
}