# Runtime Flow

## Startup

1. `Bootstrap.Program.Main` constructs `Game.Game` and passes it with `OpenGL.Core.BackendFactory` to `Engine`.
2. `Engine` constructs `Core.Window`, discovers systems, and subscribes to the window's load, update, and render events.
3. `Engine.Run()` calls the underlying SILK.NET window run loop.

## Load Callback

When SILK.NET reports that the window has loaded, `Engine.OnLoad` obtains the native GL context. It creates a `GraphicsContext` wrapping `GetProcAddress`, asks `IGraphicsBackendFactory` for an `IGraphicsBackend`, creates `TextureManager`, then calls `IGame.Initialize(graphics, textureManager)`.

This ordering matters: an OpenGL API is created only after a native context exists. Code that creates GL resources before this callback is outside the current runtime model.

## Update Callback

Each update converts the received delta time to `float` and creates `SystemContext`. `Engine` invokes every discovered `IUpdateSystem`, then calls `Game.Update`. System discovery scans loaded and entry-assembly referenced assemblies, finds non-abstract `ISystem` classes, and creates them via a parameterless constructor.

Priority metadata is collected into `SystemInfo`; the current `Engine` loop does not sort the discovered list before invoking it. Do not assume priority order without checking the code.

## Render Callback

`Engine.OnRender` clears the backend then calls `Game.Render`. The sample game's `DrawPlatform` obtains a cached backend texture handle and emits a `TextureDrawCommand` for each tile in a diagonal depth order.

## Shutdown

No explicit disposal path is currently visible in `Engine`, `Window`, or `Renderer`. Resource shutdown behavior is therefore undecided and should be designed deliberately rather than assumed.

## Related

[[Project/Architecture]] · [[Knowledge/ECS/System Discovery]] · [[Knowledge/OpenGL/Renderer Backend]]
