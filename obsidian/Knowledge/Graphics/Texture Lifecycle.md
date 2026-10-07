# Texture Lifecycle

## Mental Model

The current texture pipeline separates a named CPU image from a backend texture. A `TextureAssetHandle` identifies the former; a `BackendTextureHandle` identifies the latter.

## How It Works

1. An importer or caller produces `ImageData` with width, height, and RGBA bytes.
2. `TextureManager.Load(name, image)` records the image once per name and returns a handle containing its list index and generation.
3. Drawing code asks `TextureManager.GetBackendHandle(handle)`.
4. The manager checks index and generation. If the entry has no backend handle, it calls `IGraphicsBackend.CreateTexture(image)` and caches the returned handle.
5. A `TextureDrawCommand` carries the backend handle to the renderer.

## MiniEngine Relevance

The sample loads `tile_000` from the database, stores the returned asset handle, and gets its backend handle before drawing the tile grid. Text rendering follows the same manager path for rasterized glyph images.

## Things I Confused

`Load` is a registry operation, not necessarily a GPU upload. The upload happens lazily. There is no visible unload, reload, or backend resource disposal path, so those behaviors must be added deliberately if required.

## Related

[[Project/Assets and Content]] · [[Knowledge/OpenGL/Renderer Backend]] · [[Project/Decisions/Graphics go through engine contracts]]
