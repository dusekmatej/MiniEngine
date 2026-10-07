# Architecture

## Overview

The solution has six projects: `Bootstrap`, `Engine`, `Game`, `OpenGL`, `Database`, and `AssetPipeline`. The current dependency direction is practical rather than fully isolated: `Bootstrap` references `Engine`, `Game`, and `OpenGL`; `Game` references `Engine`, `Database`, and `OpenGL`; `OpenGL` references `Engine`; `Database` references `Engine` and `AssetPipeline`.

`Bootstrap` is the executable composition root. It creates `Game` and `BackendFactory`, then starts `Engine`. `Engine` owns the window, discovers update systems, creates graphics after the native context loads, and drives update and render callbacks. [[Project/Runtime Flow]] follows that path in detail.

## ECS

Entities are generation-based value types managed by `EntityManager`. Components are plain data structs; component-store code is present and still being integrated with the world. Systems are discovered from assemblies and `IUpdateSystem` instances receive a `SystemContext` with delta time during updates.

The public world and scene surface is skeletal. Existing docs describe intended queries, scene behavior, hierarchy handling, and world access, but those APIs are not present in the current implementation. Read [[Project/Implementation and Plan]] before treating those docs as current behavior.

## Rendering

`Engine.Graphics` defines graphics contracts, draw commands, texture handles, and helpers. `OpenGL` implements the backend with SILK.NET OpenGL, shaders, meshes, and textures. The current game receives the graphics backend and texture manager directly.

The OpenGL backend owns GL object creation and immediate drawing. `TextureManager` retains CPU image data, maps texture names to handles, and uploads to the backend lazily on first use. [[Knowledge/Graphics/Texture Lifecycle]] and [[Knowledge/OpenGL/Renderer Backend]] document the current flow.

## Windowing

`Engine.Core.Window` wraps a SILK.NET window and forwards load, update, and render events. The OpenGL API is created only after the native graphics context is available.

## Ownership

`Engine` creates and owns the window, graphics backend, and texture manager. `Game` initializes game content and issues rendering through engine graphics APIs. `Database` and `AssetPipeline` provide asset-related functionality.

The current ownership boundaries are implementation facts, not a promise that the public API is stable. The desired future boundary described in `plan.md` is intentionally tracked separately in [[Project/Implementation and Plan]].

## Related Decisions

[[Project/Decisions]]

Areas beyond these current code paths are undecided until recorded in a decision note.
