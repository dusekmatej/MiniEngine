using System.Numerics;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Environment;
using MiniEngine.Graphics.Presets;

namespace MiniEngine.Game.Rendering;

public static class Drawing
{
    private static bool _demoButtonWasPressed;



    public static int DrawWorld(
        Graphics2D graphics,
        TextureAssets textures,
        MiniEngine.Game.Environment.MainWorld world,
        IsometricPreset preset,
        Camera camera)
    {
        int drawnTiles = 0;
        float width = preset.SpriteWidth * camera.Zoom / graphics.AspectRatio;
        float height = preset.SpriteHeight * camera.Zoom;

        // One global painter order avoids seams where neighboring chunks overlap.
        int minimumDepth = world.MinimumTileX + world.MinimumTileY;
        int maximumDepth = world.MaximumTileX + world.MaximumTileY;
        for (int depth = minimumDepth; depth <= maximumDepth; depth++)
        {
            int minimumX = Math.Max(world.MinimumTileX, depth - world.MaximumTileY);
            int maximumX = Math.Min(world.MaximumTileX, depth - world.MinimumTileY);
            for (int x = minimumX; x <= maximumX; x++)
            {
                int y = depth - x;
                Vector2 position = camera.WorldToScreen(new Vector2(
                    (x - y) * preset.FootprintWidth / 2f,
                    -(x + y) * preset.FootprintHeight / 2f));
                position.X /= graphics.AspectRatio;

                if (position.X > 1f || position.X + width < -1f ||
                    position.Y > 1f || position.Y + height < -1f)
                {
                    continue;
                }
                if (!world.TryGetTile(x, y, out TileId ground, out TileId decoration))
                {
                    continue;
                }

                TextureAssetHandle texture = textures.Load(world.TileDefinitions.Get(ground).Texture);
                graphics.DrawSprite(texture, position.X, position.Y, width, height, EngineColor.White);
                if (decoration.IsValid)
                {
                    TextureAssetHandle overlay = textures.Load(world.TileDefinitions.Get(decoration).Texture);
                    graphics.DrawSprite(overlay, position.X, position.Y, width, height, EngineColor.White);
                }
                drawnTiles++;
            }
        }
        return drawnTiles;
    }

    public static void DrawChunk(
        Graphics2D graphics,
        TextureAssets textures,
        Chunk chunk,
        TileDefinitionRegistry definitions,
        IsometricPreset preset,
        Camera camera)
    {
        for (int depth = 0; depth < Chunk.ChunkSize * 2 - 1; depth++)
        {
            int minX = Math.Max(0, depth - Chunk.ChunkSize + 1);
            int maxX = Math.Min(Chunk.ChunkSize - 1, depth);

            for (int x = minX; x <= maxX; x++)
            {
                int y = depth - x;

                TileId tileId = chunk.Tiles[x, y];

                if (!tileId.IsValid)
                    continue;

                TileDefinition definition = definitions.Get(tileId);
                TextureAssetHandle texture = textures.Load(definition.Texture);

                int worldX = chunk.ChunkX * Chunk.ChunkSize + x;
                int worldY = chunk.ChunkY * Chunk.ChunkSize + y;

                float projectedX =
                    (worldX - worldY) * preset.FootprintWidth / 2f;

                float projectedY =
                    -(worldX + worldY) * preset.FootprintHeight / 2f;

                Vector2 screenPosition = camera.WorldToScreen(
                    new Vector2(projectedX, projectedY)
                );

                graphics.DrawSprite(
                    texture,
                    screenPosition.X,
                    screenPosition.Y,
                    preset.SpriteWidth * camera.Zoom,
                    preset.SpriteHeight * camera.Zoom,
                    EngineColor.White
                );
            }
        }
    }




    public static void DrawPlatform(DrawPlatformDto x)
    {
        if (x.Drawing is null || x.TileTexture is null)
            return;

        const float originX = -0.15f;
        const float originY = 0.35f;

        for (int depth = 0; depth <= (x.GridSize - 1) * 2; depth++)
        {
            int minimumX = Math.Max(0, depth - (x.GridSize - 1));
            int maximumX = Math.Min(x.GridSize - 1, depth);

            for (int gridX = minimumX; gridX <= maximumX; gridX++)
            {
                int gridY = depth - gridX;

                int tileId = x.Map[gridX, gridY];

                if (tileId != 0)
                    continue;

                float screenX = 
                    originX + (gridX - gridY) * x.Preset.FootprintWidth / 2f;
                float screenY = 
                    originY - (gridX + gridY) * x.Preset.FootprintHeight / 2f;

                x.Drawing.DrawSprite(
                    x.TileTexture.Value,
                    screenX, 
                    screenY, 
                    x.Preset.SpriteWidth, 
                    x.Preset.SpriteHeight,
                    EngineColor.White,
                    layer: 0
                );
            }
        }
    }


    public static void DrawPremadeItems(Graphics2D _drawing, FontAssetHandle? _debugFont, TextureAssetHandle? _tileTexture)
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
}