using MiniEngine.Graphics;
using MiniEngine.Graphics.Presets;


namespace MiniEngine.Game.Rendering;

public record struct DrawPlatformDto(
    Graphics2D Drawing,
    TextureAssetHandle? TileTexture,
    int GridSize,
    IsometricPreset Preset,
    TileMap Map
);