# Rendering API

MiniEngine uses a backend-neutral graphics API in the `MiniEngine.Graphics` namespace. The Engine project defines the API, while a backend project provides its implementation.

## Graphics Backend

`IGraphicsBackend` represents the operations needed by the Engine and the high-level `Graphics2D` API for one frame:

- `BeginFrame()` starts frame processing.
- `Clear()` clears the current frame.
- `DrawTexture(...)`, `DrawRectangle(...)`, and `DrawText(...)` execute the existing draw commands.
- `DrawTriangle(...)`, `DrawCircle(...)`, and `DrawLine(...)` execute the basic shape commands.
- `EndFrame()` finishes frame processing.

The Engine calls these operations around `IGame.Render()`. Draw command types contain only backend-neutral values, such as positions, sizes, transforms, colors, layer values, and `BackendTextureHandle` values.

## Draw Commands

The existing commands remain separate because they describe different 2D draw operations.

### Texture

`TextureDrawCommand` describes a textured quad:

```csharp
var command = new TextureDrawCommand(
	texture,
	x: 0.2f,
	y: 0.1f,
	width: 0.5f,
	height: 0.5f,
	rotation: 0.25f,
	scale: new Vector2(1f, 1f),
	tint: EngineColor.White,
	layer: 2
);
```

It contains the backend texture handle, position, size, rotation in radians, scale, tint, and draw layer.

### Rectangle

`RectangleDrawCommand` describes a colored rectangle. It retains the existing color property and supports the same position, size, rotation, scale, and layer data:

```csharp
var command = new RectangleDrawCommand(
	x: -0.5f,
	y: -0.25f,
	width: 1f,
	height: 0.5f,
	color: EngineColor.Blue,
	rotation: 0f,
	scale: Vector2.One,
	layer: 0
);
```

### Text

Text content and its font settings are handled by the existing `TextRenderer`. It rasterizes the requested text into glyph textures, then submits `TextDrawCommand` values for those glyphs. A text command contains the glyph texture handle, position, size, color, rotation, scale, and layer:

```csharp
var command = new TextDrawCommand(
	glyphTexture,
	x: 0f,
	y: 0f,
	width: 0.05f,
	height: 0.1f,
	color: EngineColor.White,
	rotation: 0f,
	scale: Vector2.One,
	layer: 10
);
```

The backend does not receive OpenGL behavior or handles in these commands. It receives only the data needed to execute each 2D operation.

## High-Level Drawing API

Game code uses `Graphics2D`. It describes what to draw and never calls `IGraphicsBackend`, `Renderer`, or OpenGL directly. The API translates each operation into a neutral draw command.

```csharp
graphics.DrawRectangle(-0.8f, -0.8f, 0.4f, 0.2f, EngineColor.Yellow);
graphics.DrawTriangle(0.1f, 0.2f, 0.25f, 0.25f, EngineColor.Green);
graphics.DrawCircle(0.6f, 0.4f, 0.1f, EngineColor.Cyan);
graphics.DrawLine(-0.5f, 0f, 0.5f, 0.3f, 0.02f, EngineColor.Red);
graphics.DrawSprite(texture, 0f, 0f, 0.25f, 0.25f, EngineColor.White);
```

`DrawSprite` uses the existing texture draw command and `TextureAssets` cache. Text remains available through `DrawText` and the existing glyph rasterization path. The `layer` value is carried by every command; immediate rendering preserves submission order, while the value is ready for a later ordering policy.

## Textures

`ImageData` is the neutral input used by `CreateTexture`. The backend returns a `BackendTextureHandle`, which can be stored in draw commands without exposing OpenGL handles or enums to Engine code.

When a backend texture is no longer needed, `DestroyTexture` releases the backend resource represented by the handle.

`TextureAssetHandle` is the stable Engine/Game identity of an image. `TextureAssets` keeps the CPU-side `ImageData`, asks the configured `IAssetSource` for named images when needed, and lazily creates the backend texture on the first `GetBackendHandle` call. Repeated use of the same asset returns the cached backend handle instead of uploading the image again.

```csharp
TextureAssetHandle tile = textureAssets.Load("tile_000");

drawing.DrawTexture(
	tile,
	x: 0.1f,
	y: 0.2f,
	width: 0.25f,
	height: 0.25f,
	tint: EngineColor.White
);
```

The asset source is neutral. The current Database project supplies `DatabaseAssetSource`, while the texture cache and backend handle remain in Engine and OpenGL respectively.

## Backend Creation

`GraphicsContext` carries the procedure-address loader supplied after the window and graphics context exist. `IGraphicsBackendFactory.Create` receives that context and creates the selected backend. Engine code therefore depends on the factory and neutral interfaces, while OpenGL-specific setup remains in the `OpenGL` project.

## OpenGL Implementation

After the window creates its OpenGL context, `Engine` builds a `GraphicsContext` from the context's procedure loader and passes it to the graphics factory. The OpenGL project's `BackendFactory` wraps that loader for Silk.NET, creates `GL`, and constructs the existing `Renderer`.

During rendering, `Engine` calls the neutral `IGraphicsBackend` methods. `Renderer` receives the existing draw commands and uses its OpenGL shaders, quad mesh, VAO/VBO, textures, and OpenGL state to execute them. No OpenGL type or handle crosses into Engine or Game.

The window size is captured in `GraphicsContext` after the context exists. The OpenGL renderer applies those dimensions to its viewport at the beginning of a frame. Draw commands remain in the Engine's normalized 2D coordinate space.

## Game Usage

Game receives `Graphics2D`, `TextureAssets`, and `FontManager` through `IGame.Initialize`. It describes what to draw through `Graphics2D`; it does not create OpenGL resources or call OpenGL directly:

```csharp
TextureAssetHandle tile = textureAssets.Load("tile_000");

graphics.DrawRectangle(-1f, -1f, 2f, 2f, backgroundColor);
graphics.DrawSprite(tile, 0.1f, 0.2f, 0.25f, 0.25f, EngineColor.White);
graphics.DrawTriangle(0.45f, -0.35f, 0.25f, 0.25f, EngineColor.Green);
graphics.DrawCircle(0.72f, 0.30f, 0.10f, EngineColor.Cyan);
graphics.DrawLine(0.4f, -0.7f, 0.85f, -0.55f, 0.025f, EngineColor.Red);
graphics.DrawText(font, "MiniEngine Rendering", -0.8f, 0.8f, 0.1f, EngineColor.White);
```

Bootstrap selects `BackendFactory` and supplies the Database asset source. Engine owns the neutral contracts and frame lifecycle. Game owns the requested scene/game draw commands. OpenGL owns `GL`, shaders, meshes, textures, handles, and command execution.