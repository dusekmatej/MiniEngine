# Text Rendering

## Mental Model

Text is currently rendered one glyph at a time. A font file is loaded into memory, a character is rasterized into an alpha bitmap, that bitmap becomes a texture, and a text draw command tints it.

## How It Works

`FontManager` maps font names to `FontAssetHandle` values and reads font bytes. `TextRenderer` rasterizes at a fixed 64-pixel height and caches each `(font, character)` result. For visible glyphs it converts the alpha bitmap into white RGBA pixels, loads them through `TextureManager`, and caches the resulting texture handle with offsets and advance.

`DrawText` scales raster metrics to the requested height, emits one `TextDrawCommand` per glyph, and advances the cursor by the glyph advance. The OpenGL text shader reads the texture alpha and applies the command color.

## MiniEngine Relevance

The sample registers `Game/Assets/JetBrainsMono-Bold.ttf` as `debug` and includes a font rasterization test. This path is useful for understanding asset handles and texture caching, even if the surrounding sample code is changing.

## Things I Confused

The cached glyph image is not an atlas. The current implementation may create a separate backend texture for each encountered visible glyph.

## Related

[[Project/Assets and Content]] · [[Knowledge/Graphics/Texture Lifecycle]]
