# Codebase Map

## Entry Points

`Bootstrap/Program.cs` is the executable entry point. It creates `MiniEngine.Game.Game`, supplies `MiniEngine.OpenGL.Core.BackendFactory`, and calls `Engine.Run()`.

## Projects

| Project | Current responsibility | Useful first files |
| --- | --- | --- |
| `Bootstrap/` | Application startup and composition | `Program.cs` |
| `Engine/` | Runtime loop, graphics contracts, ECS pieces, environment types | `Engine.cs`, `Core/Window.cs` |
| `OpenGL/` | SILK.NET OpenGL renderer implementation | `Core/BackendFactory.cs`, `Core/Renderer.cs` |
| `Game/` | Current sample content and rendering exercise | `Game.cs` |
| `Database/` | Typed in-memory storage and terrain import | `Database.cs`, `Import/TerrainImport.cs` |
| `AssetPipeline/` | Image decoding into `ImageData` | `ImageLoader.cs` |

## Engine Areas

- `Engine/Core/` holds `Window`, `IGame`, and raw `ImageData`.
- `Engine/Graphics/` holds backend interfaces, draw commands, texture/font management, tile-map, camera, and isometric projection types.
- `Engine/Entity/` contains the entity handle and lifetime manager.
- `Engine/Components/` contains component stores, their shared store interface, and data-only component structs.
- `Engine/Systems/` contains system discovery and priority support.
- `Engine/Environment/` contains world, scene, chunk, and tile types. Its public APIs are still minimal.

## First-Read Paths

- Startup or window issue: [[Project/Runtime Flow]], then `Bootstrap/Program.cs`, `Engine/Engine.cs`, and `Engine/Core/Window.cs`.
- Rendering issue: [[Knowledge/OpenGL/Renderer Backend]], then `OpenGL/Core/Renderer.cs` and the relevant `Engine/Graphics/` type.
- ECS issue: [[Knowledge/ECS/Component Stores]], then `Engine/Entity/` and `Engine/Components/`.
- Asset issue: [[Project/Assets and Content]], then `Database/Import/TerrainImport.cs` and `AssetPipeline/ImageLoader.cs`.
