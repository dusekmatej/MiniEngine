using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Graphics;
using MiniEngine.Core;
using MiniEngine.Game.Rendering;
using MiniEngine.Game.Environment;
using System.Numerics;
using Silk.NET.Input;

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
    private readonly Camera _camera = new(new Vector3(0f, -1.125f, 0f));
    private IKeyboard? _keyboard;
    private const float CameraSpeed = 2f;

    public Game()
        => _preset = IsometricPreset.Default;

    public void Initialize(
        Graphics2D graphics,
        TextureAssets textureAssets,
        FontManager fontManager,
        IInputContext input)
    {
        _textureAssets = textureAssets;
        _drawing = graphics;
        _fontManager = fontManager;

        if (input.Keyboards.Count > 0)
            _keyboard = input.Keyboards[0];

        if (input.Mice.Count > 0)
            input.Mice[0].Scroll += OnMouseScroll;


        Console.WriteLine("Game: Initializing game...");

        _tileTexture = textureAssets.Load("Terrain/Grass/tile_040");
        
        InitializeFont();
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
    {
        _camera.Zoom *= MathF.Pow(1.1f, scroll.Y);
    }

    public void Update(float deltaTime)
    {
        if (_keyboard is null)
            return;

        Vector3 movement = Vector3.Zero;

        if (_keyboard.IsKeyPressed(Key.W))
            movement.Y += 1f;

        if (_keyboard.IsKeyPressed(Key.S))
            movement.Y -= 1f;

        if (_keyboard.IsKeyPressed(Key.A))
            movement.X -= 1f;

        if (_keyboard.IsKeyPressed(Key.D))
            movement.X += 1f;

        if (movement != Vector3.Zero)
        {
            movement = Vector3.Normalize(movement);

            _camera.Position += movement * CameraSpeed * deltaTime;
        }
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

            Drawing.DrawChunk(_drawing, _textureAssets, world.Chunk, world.TileDefinitions, _preset, _camera);
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
