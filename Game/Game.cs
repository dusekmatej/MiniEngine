using System.Numerics;
using MiniEngine.Environment;
using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Graphics;
using MiniEngine.Core;
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
    private bool _demoButtonWasPressed;

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

        _tileTexture = textureAssets.Load("tile_000");

        string fontPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "JetBrainsMono-Bold.ttf"
        );

        _debugFont = _fontManager.AddFont(
            "debug",
            fontPath
        );

        TileDefinitionsTest();
        
        Entity testEntity = new Entity();
    }

    public void Update(float deltaTime)
    {
    }

    public void Render()
    {
        if (_drawing is null || _debugFont is null)
            throw new Exception("Game: Drawing or debug font is not initialized.");

        DrawPremadeItems();
    }

    private void DrawPremadeItems()
    {
        if (_drawing is null || _debugFont is null)
            return;

        _drawing.DrawRectangle(
            -1f,
            -1f,
            2f,
            2f,
            EngineColor.FromNormalized(0.08f, 0.09f, 0.14f),
            layer: -10
        );

        _drawing.DrawText(
            _debugFont.Value,
            "DrawCommands demo",
            -0.88f,
            0.82f,
            0.10f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Triangle",
            -0.88f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawTriangle(
            -0.78f,
            0.25f,
            0.22f,
            0.22f,
            EngineColor.Green,
            rotation: 0.18f,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Circle",
            -0.52f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawCircle(
            -0.40f,
            0.35f,
            0.10f,
            EngineColor.Cyan,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Line",
            -0.15f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawLine(
            -0.15f,
            0.33f,
            0.10f,
            0.33f,
            0.025f,
            EngineColor.Red,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Rectangle",
            0.18f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawRectangle(
            0.28f,
            0.25f,
            0.25f,
            0.18f,
            EngineColor.Yellow,
            rotation: 0.12f,
            scale: new Vector2(1f, 0.85f),
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Outline",
            0.58f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawRectangleOutline(
            new Rectangle(0.62f, 0.25f, 0.22f, 0.18f),
            EngineColor.Magenta,
            thickness: 0.02f,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Circle outline",
            -0.88f,
            -0.02f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawCircle(
            -0.70f,
            -0.18f,
            0.10f,
            EngineColor.Magenta,
            filled: false,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Sprite",
            -0.35f,
            -0.02f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        if (_tileTexture is TextureAssetHandle texture)
        {
            _drawing.DrawSprite(
                texture,
                -0.28f,
                -0.20f,
                0.20f,
                0.20f,
                EngineColor.White,
                layer: 2
            );
        }

        _drawing.DrawText(
            _debugFont.Value,
            "Button",
            0.12f,
            -0.02f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        Rectangle buttonBounds = new Rectangle(0.10f, -0.25f, 0.48f, 0.20f);
        bool buttonPressed = _drawing.DrawButton(
            buttonBounds,
            "Click me",
            EngineColor.FromRgb(20, 90, 190),
            EngineColor.White,
            _debugFont.Value
        );

        if (buttonPressed && !_demoButtonWasPressed)
            Console.WriteLine("Hello");

        _demoButtonWasPressed = buttonPressed;
    }

    private void DrawPlatform()
    {
        if (_drawing is null || _tileTexture is null)
            return;

        const float originX = -0.15f;
        const float originY = 0.35f;

        for (int depth = 0; depth <= (GridSize - 1) * 2; depth++)
        {
            int minimumX = Math.Max(0, depth - (GridSize - 1));
            int maximumX = Math.Min(GridSize - 1, depth);

            for (int gridX = minimumX; gridX <= maximumX; gridX++)
            {
                int gridY = depth - gridX;

                int tileId = _map[gridX, gridY];

                if (tileId != 0)
                    continue;

                float screenX = 
                    originX + (gridX - gridY) * _preset.FootprintWidth / 2f;
                float screenY = 
                    originY - (gridX + gridY) * _preset.FootprintHeight / 2f;

                _drawing.DrawSprite(
                    _tileTexture.Value,
                    screenX, 
                    screenY, 
                    _preset.SpriteWidth, 
                    _preset.SpriteHeight,
                    EngineColor.White,
                    layer: 0
                );
            }
        }
    }

    private void TileDefinitionsTest()
    {
        var tileDef = new TileDefinitionRegistry();

        var grassId = tileDef.Register(new TileDefinition("Grass"));
        var dirtId = tileDef.Register(new TileDefinition("Dirt"));

        var grassTile = new Tile(grassId);
        var dirtTile = new Tile(dirtId, 2);
        
        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine($"Grass ID: {grassTile.DefinitionId.Value}, key: {tileDef.Get(grassTile.DefinitionId)?.Key}");
        Console.WriteLine($"Dirt ID: {dirtTile.DefinitionId.Value}, key: {tileDef.Get(dirtTile.DefinitionId)?.Key}, variation: {dirtTile.Variation}");
        Console.WriteLine($"Grass valid: {grassTile.DefinitionId.IsValid}");
        Console.WriteLine($"Dirt valid: {dirtTile.DefinitionId.IsValid}");
        Console.WriteLine("-----------------------------------------------");
    }

}