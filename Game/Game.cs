using System.Numerics;
using MiniEngine.Environment;
using MiniEngine.Database.Import;
using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Graphics;
using MiniEngine.Core;

namespace MiniEngine.Game;

public class Game : IGame
{
    private const int GridSize = 9;
    private static readonly Vector2 TerrainScreenOrigin = new Vector2(-0.15f, 0.35f);
    
    private readonly FontManager _fontManager = new();
    private TextureAssetHandle? _glyphTexture;
    private float _glyphDrawWidth;
    private float _glyphDrawHeight;

    private Graphics2D? _drawing;
    private FontAssetHandle? _debugFont;

    private readonly RasterizeFont _rasterizer;

    private TextureAssetHandle? _tileTexture;

    private TextureManager? _textureManager;
    private IGraphicsBackend? _graphics;

    private IsometricPreset _preset = IsometricPreset.Default;
    private IsometricProjection _projection;
    private Camera _camera;
    private TileMap _map = new TileMap(GridSize, GridSize);

    public Game()
    {
        _preset = IsometricPreset.Default;
        _projection = new IsometricProjection(_preset);
        _camera = new Camera(Vector3.Zero);
      }

    public void Initialize(IGraphicsBackend graphics, TextureManager textureManager)
    {
        _graphics = graphics;
        _textureManager = textureManager;
        _drawing = new Graphics2D(graphics, textureManager, _fontManager);

        Console.WriteLine("Game: Initializing game...");

        TerrainImport.PopulateDatabase();

        var tile = global::MiniEngine.Database.Database.Get<ImageData>("tile_000");

        if (tile is null)
        {
            Console.WriteLine("Game: Failed to retrieve image 'tile_000' from the database."); 
            
            return; 
        }

        _tileTexture = _textureManager.Load("tile_000", tile);

        string fontPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "JetBrainsMono-Bold.ttf"
        );

        _debugFont = _fontManager.AddFont(
            "debug",
            fontPath
        );

        // Tests
        TileDefinitionsTest();
        FontTest();
    }

    public void Update(float deltaTime)
    {
    }

    public void Render()
    {
        DrawPlatform();

        if (_drawing is null || _debugFont is null)
            throw new Exception("Game: Drawing or debug font is not initialized.");

            _drawing.DrawText(_debugFont.Value, "Hello, MiniEngine!", -0.85f, 0.75f, 0.15f, EngineColor.White);
    }

    private void DrawPlatform()
    {
        if (_textureManager is null || _graphics is null || _tileTexture is null)
            return;

        var backendTexture = _textureManager.GetBackendHandle(_tileTexture.Value);

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

                var drawCommand = new TextureDrawCommand(
                    backendTexture, 
                    screenX, 
                    screenY, 
                    _preset.SpriteWidth, 
                    _preset.SpriteHeight
                );
                
                _graphics.DrawTexture(drawCommand);
                //_graphics.DrawRectangle(new RectangleDrawCommand(
                  //  screenX, 
                    //screenY, 
                    //_preset.SpriteWidth, 
                    //_preset.SpriteHeight, 
                    //EngineColor.Yellow
                //));
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

    private void DrawGlyphTest()
    {
        if (_textureManager is null || _graphics is null || _glyphTexture is null)
            return;

        var backendTexture =
            _textureManager.GetBackendHandle(_glyphTexture.Value);

        var drawCommand = new TextureDrawCommand(
            backendTexture,
            -0.85f,
            0.55f,
            _glyphDrawWidth,
            _glyphDrawHeight
        );

        _graphics.DrawTexture(drawCommand);
    }

    private void FontTest()
    {
        if (_textureManager is null)
            return;



        var rasterizer = new RasterizeFont(_fontManager);

        var glyph = rasterizer.RasterizeGlyph(
            _debugFont.Value,
            'A',
            32f
        );

        Console.WriteLine(
            $"Glyph '{glyph.Character}'\n" +
            $"Size: {glyph.Width}x{glyph.Height}\n" +
            $"Advance: {glyph.Advance}\n" +
            $"Pixels: {glyph.pixels.Length}"
        );

        ImageData glyphImage = CreateGlyphImage(glyph);

        _glyphTexture = _textureManager.Load(
            "glyph_A",
            glyphImage
        );

        const float desiredHeight = 0.20f;

        _glyphDrawHeight = desiredHeight;
        _glyphDrawWidth = glyph.Height > 0
            ? desiredHeight * glyph.Width / (float)glyph.Height
            : desiredHeight;
    }

    private ImageData CreateGlyphImage(RasterizedGlyph glyph)
    {
    byte[] rgbaPixels = new byte[glyph.Width * glyph.Height * 4];

    for (int i = 0; i < glyph.pixels.Length; i++)
    {
        byte alpha = glyph.pixels[i];
        int pixelIndex = i * 4;

        rgbaPixels[pixelIndex + 0] = 255;   // R
        rgbaPixels[pixelIndex + 1] = 255;   // G
        rgbaPixels[pixelIndex + 2] = 255;   // B
        rgbaPixels[pixelIndex + 3] = alpha; // A
    }

    return new ImageData(
        glyph.Width,
        glyph.Height,
        rgbaPixels
    );
}

}