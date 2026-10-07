# Graphics go through engine contracts

## Decision

Current rendering code exposes `IGraphicsBackend`, `IGraphicsBackendFactory`, graphics context, draw commands, and texture handles from `Engine.Graphics`; the OpenGL project implements the backend.

## Why

The README names rendering abstraction as an active area, and the current source uses these interfaces between `Engine`, `Game`, and `OpenGL`. This records the present boundary, not a claim that the boundary is complete.

## Alternatives

`plan.md` proposes a stricter backend-neutral split in which game code does not receive renderer or database dependencies. That proposal has not replaced the current `IGame.Initialize(IGraphicsBackend, TextureManager)` contract.

## Consequences

Backend-specific GL calls belong in `OpenGL/`. Changes to graphics interfaces affect the engine and current game integration. Keep `TextureAssetHandle` distinct from `BackendTextureHandle`.

## Related

[[Knowledge/OpenGL/Renderer Backend]] · [[Knowledge/Graphics/Texture Lifecycle]] · [[Project/Implementation and Plan]]
