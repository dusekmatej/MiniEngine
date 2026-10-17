using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Graphics;
using MiniEngine.Core;
using MiniEngine.Game.Rendering;
using MiniEngine.Game.Environment;

namespace MiniEngine.Game;

public class Game : IGame
{
    private Graphics2D? _drawing;
    private FontManager? _fontManager;
    private FontAssetHandle? _debugFont;

    private TextureAssetHandle? _tileTexture;
    private TextureAssets? _textureAssets;

    private IsometricPreset _preset = IsometricPreset.Default;

    private readonly MainScene _mainScene = new();

    public Game()
        => _preset = IsometricPreset.Default;

    public void Initialize(
        Graphics2D graphics,
        TextureAssets textureAssets,
        FontManager fontManager)
    {
        _textureAssets = textureAssets;
        _drawing = graphics;
        _fontManager = fontManager;

        Console.WriteLine("Game: Initializing game...");

        _tileTexture = textureAssets.Load("Terrain/Grass/tile_040");
        
        InitializeFont();
    }

    public void Update(float deltaTime)
    {
    }

    public void Render()
    {
        var world = _mainScene.MainWorld;

        if (_drawing is null || _debugFont is null)
            throw new Exception("Game: Drawing or debug font is not initialized.");

        // Drawing.DrawPremadeItems(_drawing, _debugFont, _tileTexture);

        DrawPlatformDto _drawPlatformDto = new DrawPlatformDto(
            _drawing,
            _tileTexture,
            MainWorld.GridSize,
            _preset,
            world.Map
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
