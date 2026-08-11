using MiniEngine.Database.Import;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Presets;
using MiniEngine.Core;

namespace MiniEngine.Game;

public class Game : IGame
{
    private TextureAssetHandle? _tileTexture;

    private TextureManager? _textureManager;
    private IGraphicsBackend? _graphics;

    private IsometricPreset _preset = IsometricPreset.Default;

    private TileMap? _map;

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

        _map = new TileMap(9, 9);

        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                _map[x, y] = 0;
            }
        }

        Console.WriteLine(
            $"Game: Successfully retrieved image 'tile_000' from the database. " +
            $"Dimensions: {tile.Width}x{tile.Height}");
    }

    public void Update(float deltaTime)
    {
    }

    public void Render()
    {
        if (_tileTexture is null || _textureManager is null || _graphics is null)
            return;

        var backendTexture = 
            _textureManager.GetBackendHandle(_tileTexture.Value);

        const int gridSize = 9;
        
        const float originX = -0.15f;
        const float originY = 0.35f;

        for (int depth = 0; depth <= (gridSize - 1) * 2; depth++)
        {
            int minimumX = Math.Max(0, depth - (gridSize - 1));
            int maximumX = Math.Min(gridSize - 1, depth);

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