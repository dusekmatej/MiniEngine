using MiniEngine.Core;
using System.Numerics;

namespace MiniEngine.Graphics.Fonts;

internal sealed class TextRenderer
{
    private const float RasterHeight = 64f;

    private readonly IGraphicsBackend _graphics;
    private readonly TextureAssets _textureAssets;
    private readonly RasterizeFont _rasterizer;

    private readonly Dictionary<
        (FontAssetHandle Font, char Character),
        CachedGlyph> _glyphs = new();

    public TextRenderer(
        IGraphicsBackend graphics,
        TextureAssets textureAssets,
        FontManager fontManager)
    {
        _graphics = graphics;
        _textureAssets = textureAssets;

        _rasterizer = new RasterizeFont(
            fontManager
        );
    }

    public void DrawText(
        FontAssetHandle font,
        string text,
        float x,
        float y,
        float height,
        EngineColor color,
        float rotation,
        Vector2 commandScale,
        int layer)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (height <= 0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                "Text height must be greater than zero."
            );
        }

        float rasterScale =
            height / RasterHeight;

        float cursorX = x;

        foreach (char character in text)
        {
            CachedGlyph glyph =
                GetGlyph(font, character);

            if (glyph.Texture is not null)
            {
                BackendTextureHandle backendTexture =
                    _textureAssets.GetBackendHandle(
                        glyph.Texture.Value
                    );

                float drawX =
                    cursorX +
                    glyph.OffsetX * rasterScale;

                float drawY =
                    y -
                    (glyph.OffsetY + glyph.Height) * rasterScale;

                float drawWidth =
                    glyph.Width * rasterScale;

                float drawHeight =
                    glyph.Height * rasterScale;

                var command =
                    new TextDrawCommand(
                        backendTexture,
                        drawX,
                        drawY,
                        drawWidth,
                        drawHeight,
                        color,
                        rotation,
                        commandScale,
                        layer
                    );

                _graphics.DrawText(command);
            }

            cursorX +=
                glyph.Advance * rasterScale;
        }
    }

    private CachedGlyph GetGlyph(
        FontAssetHandle font,
        char character)
    {
        var key =
            (font, character);

        if (_glyphs.TryGetValue(
                key,
                out CachedGlyph cachedGlyph))
        {
            return cachedGlyph;
        }

        RasterizedGlyph rasterized =
            _rasterizer.RasterizeGlyph(
                font,
                character,
                RasterHeight
            );

        TextureAssetHandle? texture = null;

        if (rasterized.Width > 0 &&
            rasterized.Height > 0 &&
            rasterized.pixels.Length > 0)
        {
            ImageData image =
                CreateGlyphImage(rasterized);

            texture =
                _textureAssets.Load(
                    $"font_{font.Index}_glyph_{(int)character}",
                    image
                );
        }

        var glyph = new CachedGlyph(
            texture,
            rasterized.Width,
            rasterized.Height,
            rasterized.OffsetX,
            rasterized.OffsetY,
            rasterized.Advance
        );

        _glyphs.Add(
            key,
            glyph
        );

        return glyph;
    }

    private static ImageData CreateGlyphImage(
        RasterizedGlyph glyph)
    {
        byte[] pixels =
            new byte[
                glyph.Width *
                glyph.Height *
                4
            ];

        for (int i = 0;
             i < glyph.pixels.Length;
             i++)
        {
            byte alpha =
                glyph.pixels[i];

            int index = i * 4;

            pixels[index] = 255;
            pixels[index + 1] = 255;
            pixels[index + 2] = 255;
            pixels[index + 3] = alpha;
        }

        return new ImageData(
            glyph.Width,
            glyph.Height,
            pixels
        );
    }
}