using System.Numerics;
using MiniEngine.Database.Import;
using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics;
using MiniEngine.Core;

namespace MiniEngine.Game;

public class Game : IGame
{
    private const int GridSize = 9;
    private static readonly Vector2 TerrainScreenOrigin = new Vector2(-0.15f, 0.35f);
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

        Console.WriteLine("Game: Initializing game...");

        TerrainImport.PopulateDatabase();

        var tile = global::MiniEngine.Database.Database.Get<ImageData>("tile_000");

        if (tile is null)
        {
            Console.WriteLine("Game: Failed to retrieve image 'tile_000' from the database."); 
            
            return;
        }

        _tileTexture = _textureManager.Load("tile_000", tile);
    }

    public void Update(float deltaTime)
    {
    }

    public void Render()
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
                
                _graphics.DrawTexture(
                    backendTexture, 
                    screenX, 
                    screenY, 
                    _preset.SpriteWidth, 
                    _preset.SpriteHeight
                    );
            }
        }

    }

}