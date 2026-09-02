using MiniEngine.Core;

namespace MiniEngine.Graphics.Fonts;

internal sealed class TextRenderer
{
    private const float RasterHeight = 64f;

    private readonly IGraphicsBackend _graphics;
    private readonly TextureManager _textureManager;
    private readonly RasterizeFont _rasterizer;

    private readonly Dictionary<
        (FontAssetHandle Font, char Character),
        CachedGlyph> _glyphs = new();

    public TextRenderer(
        IGraphicsBackend graphics,
        TextureManager textureManager,
        FontManager fontManager)
    {
        _graphics = graphics;
        _textureManager = textureManager;

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
        EngineColor color)
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

        float scale =
            height / RasterHeight;

        float cursorX = x;

        foreach (char character in text)
        {
            CachedGlyph glyph =
                GetGlyph(font, character);

            if (glyph.Texture is not null)
            {
                BackendTextureHandle backendTexture =
                    _textureManager.GetBackendHandle(
                        glyph.Texture.Value
                    );

                float drawX =
                    cursorX +
                    glyph.OffsetX * scale;

                float drawY =
                    y -
                    (glyph.OffsetY + glyph.Height) * scale;

                float drawWidth =
                    glyph.Width * scale;

                float drawHeight =
                    glyph.Height * scale;

                var command =
                    new TextDrawCommand(
                        backendTexture,
                        drawX,
                        drawY,
                        drawWidth,
                        drawHeight,
                        color
                    );

                _graphics.DrawText(command);
            }

            cursorX +=
                glyph.Advance * scale;
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
                _textureManager.Load(
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