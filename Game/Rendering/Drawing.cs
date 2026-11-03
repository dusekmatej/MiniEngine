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
        TextureAssetHandle cliffTexture = textures.Load("Terrain/isometric_assets/stone2");
        TextureAssetHandle dirtTexture = textures.Load("Terrain/isometric_assets/dirt1");

        // One global painter order keeps front columns in front of terrain behind them.
        int minimumDepth = world.MinimumTileX + world.MinimumTileY;
        int maximumDepth = world.MaximumTileX + world.MaximumTileY;
        for (int depth = minimumDepth; depth <= maximumDepth; depth++)
        {
            int minimumX = Math.Max(world.MinimumTileX, depth - world.MaximumTileY);
            int maximumX = Math.Min(world.MaximumTileX, depth - world.MinimumTileY);
            for (int x = minimumX; x <= maximumX; x++)
            {
                int y = depth - x;
                if (!world.TryGetTile(x, y, out TileId ground, out int height))
                {
                    continue;
                }

                int leftHeight = world.GetHeight(x, y + 1);
                int rightHeight = world.GetHeight(x + 1, y);
                string texturePath = world.TileDefinitions.Get(ground).Texture;
                TextureAssetHandle texture = textures.Load(texturePath);
                if (DrawTerrainTile(graphics, texture, cliffTexture, dirtTexture, texturePath, x, y, height, leftHeight, rightHeight, preset, camera))
                {
                    drawnTiles++;
                }
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
        TextureAssetHandle cliffTexture = textures.Load("Terrain/isometric_assets/stone2");
        TextureAssetHandle dirtTexture = textures.Load("Terrain/isometric_assets/dirt1");
        for (int depth = 0; depth < Chunk.ChunkSize * 2 - 1; depth++)
        {
            int minimumX = Math.Max(0, depth - Chunk.ChunkSize + 1);
            int maximumX = Math.Min(Chunk.ChunkSize - 1, depth);
            for (int x = minimumX; x <= maximumX; x++)
            {
                int y = depth - x;
                TileId tile = chunk.Tiles[x, y];
                if (!tile.IsValid)
                {
                    continue;
                }

                int worldX = chunk.ChunkX * Chunk.ChunkSize + x;
                int worldY = chunk.ChunkY * Chunk.ChunkSize + y;
                int frontX = 0;
                int frontY = 0;
                if (x + 1 < Chunk.ChunkSize)
                {
                    frontX = chunk.Heights[x + 1, y];
                }
                if (y + 1 < Chunk.ChunkSize)
                {
                    frontY = chunk.Heights[x, y + 1];
                }

                string texturePath = definitions.Get(tile).Texture;
                TextureAssetHandle texture = textures.Load(texturePath);
                DrawTerrainTile(graphics, texture, cliffTexture, dirtTexture, texturePath, worldX, worldY, chunk.Heights[x, y],
                    frontY, frontX, preset, camera);
            }
        }
    }

    private static bool DrawTerrainTile(
        Graphics2D graphics,
        TextureAssetHandle surfaceTexture,
        TextureAssetHandle cliffTexture,
        TextureAssetHandle dirtTexture,
        string texturePath,
        int worldX,
        int worldY,
        int height,
        int leftHeight,
        int rightHeight,
        IsometricPreset preset,
        Camera camera)
    {
        Vector2 position = camera.WorldToScreen(new Vector2(
            (worldX - worldY) * preset.FootprintWidth / 2f,
            -(worldX + worldY) * preset.FootprintHeight / 2f));
        position.X /= graphics.AspectRatio;

        float width = preset.FootprintWidth * camera.Zoom / graphics.AspectRatio;
        float footprintHeight = preset.FootprintHeight * camera.Zoom;
        float elevationStep = preset.ElevationStep * camera.Zoom;
        float surfaceY = position.Y + height * elevationStep;
        int lowestVisibleLevel = Math.Min(height, Math.Min(leftHeight, rightHeight));
        float bottomY = position.Y + lowestVisibleLevel * elevationStep;
        if (position.X > 1f || position.X + width < -1f ||
            bottomY > 1f || surfaceY + footprintHeight < -1f)
        {
            return false;
        }

        Vector2 towardsFront = new(width / 2f, -footprintHeight / 2f);
        Vector2 towardsBack = new(width / 2f, footprintHeight / 2f);
        Vector2 down = new(0f, -elevationStep);
        bool isWater = texturePath.StartsWith("Terrain/Water/", StringComparison.Ordinal);
        EngineColor surfaceTint = GetSurfaceTint(texturePath);

        if (!isWater)
        {
            // Sample the existing side faces, but project them as full cubes rather than slabs.
            for (int level = lowestVisibleLevel; level < height; level++)
            {
                float topY = position.Y + (level + 1) * elevationStep;
                if (topY - elevationStep > 1f || topY + footprintHeight / 2f < -1f)
                {
                    continue;
                }
                bool isTopBlock = level == height - 1;
                TextureAssetHandle sideTexture = isTopBlock ? surfaceTexture : cliffTexture;
                bool grassEdge = isTopBlock && texturePath.Contains("/grass", StringComparison.Ordinal);
                if (grassEdge)
                {
                    sideTexture = dirtTexture;
                }
                EngineColor leftTint = EngineColor.FromNormalized(0.68f, 0.71f, 0.73f);
                EngineColor rightTint = EngineColor.FromNormalized(0.86f, 0.88f, 0.90f);
                if (level >= leftHeight)
                {
                    DrawBlockSide(graphics, sideTexture, surfaceTexture, grassEdge,
                        new Vector2(position.X, topY + footprintHeight / 2f), towardsFront, down,
                        new Vector2(0.04f, 0.46f), new Vector2(0.45f, 0.35f), leftTint);
                }
                if (level >= rightHeight)
                {
                    DrawBlockSide(graphics, sideTexture, surfaceTexture, grassEdge,
                        new Vector2(position.X + width / 2f, topY), towardsBack, down,
                        new Vector2(0.51f, 0.81f), new Vector2(0.45f, -0.35f), rightTint);
                }
            }
        }

        if (surfaceY <= 1f && surfaceY + footprintHeight >= -1f)
        {
            Vector2 textureOrigin = new(0.04f, 0.44f);
            Vector2 textureFront = new(0.46f, 0.35f);
            Vector2 textureBack = new(0.46f, -0.35f);
            if (isWater)
            {
                textureOrigin = new Vector2(0.04f, 0.625f);
                textureFront = new Vector2(0.46f, 0.25f);
                textureBack = new Vector2(0.46f, -0.25f);
            }
            graphics.DrawTexturedQuad(surfaceTexture,
                new Vector2(position.X, surfaceY + footprintHeight / 2f), towardsFront, towardsBack,
                textureOrigin, textureFront, textureBack, surfaceTint);
        }
        return true;
    }

    private static void DrawBlockSide(
        Graphics2D graphics,
        TextureAssetHandle sideTexture,
        TextureAssetHandle surfaceTexture,
        bool grassEdge,
        Vector2 origin,
        Vector2 horizontal,
        Vector2 down,
        Vector2 textureOrigin,
        Vector2 textureHorizontal,
        EngineColor tint)
    {
        graphics.DrawTexturedQuad(sideTexture, origin, horizontal, down,
            textureOrigin, textureHorizontal, new Vector2(0f, 0.13f), tint);
        if (grassEdge)
        {
            // Keep the grassy rim thin instead of stretching it over the whole dirt face.
            graphics.DrawTexturedQuad(surfaceTexture, origin, horizontal, down * 0.2f,
                textureOrigin, textureHorizontal, new Vector2(0f, 0.025f), tint);
        }
    }

    private static EngineColor GetSurfaceTint(string texturePath)
    {
        if (texturePath.Contains("/grass", StringComparison.Ordinal))
        {
            return EngineColor.FromNormalized(0.73f, 0.85f, 0.68f);
        }
        if (texturePath.Contains("/stone", StringComparison.Ordinal))
        {
            return EngineColor.FromNormalized(0.77f, 0.85f, 0.94f);
        }
        return EngineColor.White;
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
