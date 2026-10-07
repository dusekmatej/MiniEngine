# Assets and Content

## Current Terrain Path

`Database/Terrain/` contains PNG tile assets. `Database.csproj` copies `Terrain/**/*.png` to the output directory under `Database/Terrain/`. At startup, `Game.Initialize` calls `TerrainImport.PopulateDatabase()`.

`TerrainImport` scans the output directory for `.png`, `.jpg`, `.jpeg`, `.bmp`, and `.gif`, then uses `AssetPipeline.ImageLoader` to decode each file into `Engine.Core.ImageData`. Each image is placed in the static typed `Database` under its filename without extension; the sample requests `tile_000`.

## Texture Path

`TextureManager.Load(name, image)` creates or returns a generation-bearing `TextureAssetHandle`. It does not create a GPU texture immediately. `GetBackendHandle(handle)` validates the handle and calls `IGraphicsBackend.CreateTexture(image)` only when no backend handle exists yet.

This gives the current system one CPU image per texture name and one deferred backend upload per loaded texture entry. There is no visible unload or replacement API.

## Fonts

`Game/Assets/JetBrainsMono-Bold.ttf` is copied to the game output directory. `FontManager` reads font bytes and returns a `FontAssetHandle`. `TextRenderer` rasterizes glyphs at a fixed height, converts alpha pixels into RGBA images, loads them through `TextureManager`, and caches glyph information by `(font, character)`.

## Important Limits

`Database` is a static, typed in-memory store. Duplicate keys throw, and `Get` throws for a missing key. Asset import is currently triggered by `Game`, so game code is aware of the database. `plan.md` proposes moving that wiring to Bootstrap, but that has not happened.

## Related

[[Knowledge/Graphics/Texture Lifecycle]] · [[Knowledge/Graphics/Text Rendering]] · [[Project/Implementation and Plan]]
