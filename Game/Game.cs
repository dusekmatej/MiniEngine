using System.Numerics;
using MiniEngine.Environment;
using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Graphics;
using MiniEngine.Core;
using MiniEngine.Game.Rendering;
using MiniEngine.Entities;

namespace MiniEngine.Game;

public class Game : IGame
{
    private const int GridSize = 9;
    private Graphics2D? _drawing;
    private FontManager? _fontManager;
    private FontAssetHandle? _debugFont;

    private TextureAssetHandle? _tileTexture;
    private TextureAssets? _textureAssets;

    private IsometricPreset _preset = IsometricPreset.Default;
    private TileMap _map = new TileMap(GridSize, GridSize);

    public Game()
    {
        _preset = IsometricPreset.Default;
    }

    public void Initialize(
        Graphics2D graphics,
        TextureAssets textureAssets,
        FontManager fontManager)
    {
        _textureAssets = textureAssets;
        _drawing = graphics;
        _fontManager = fontManager;

        Console.WriteLine("Game: Initializing game...");

        _tileTexture = textureAssets.Load("tile_022");

        InitializeFont();
    }

    public void Update(float deltaTime)
    {
    }

    public void Render()
    {
        if (_drawing is null || _debugFont is null)
            throw new Exception("Game: Drawing or debug font is not initialized.");

        // Drawing.DrawPremadeItems(_drawing, _debugFont, _tileTexture);

        DrawPlatformDto _drawPlatformDto = new DrawPlatformDto(
            _drawing,
            _tileTexture,
            GridSize,
            _preset,
            _map
        );

        Drawing.DrawPlatform(_drawPlatformDto);
    }

    private void InitializeFont()
    {
        string fontPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Fonts",
            "JetBrainsMono-Bold.ttf"
        );

        _debugFont = _fontManager.AddFont(
            "debug",
            fontPath
        );
    }
}
