# Lightweight Isometric ECS Architecture

## Summary

Refactor MiniEngine into a lightweight, data-oriented, isometric-first ECS. Game code works with entities, components, scenes, and assets—never OpenGL. Add exactly one project, `Graphics.OpenGL`, and keep the demo runnable after every completed milestone.

Create root `init.md` first as the persistent architecture context, recording the vision, dependency rules, ECS decisions, namespace map, milestone status, and the rule that OpenGL never enters game-facing APIs.

## Target Architecture

| Project | Responsibility and dependencies |
|---|---|
| `Engine` | Backend-neutral runtime, ECS, scenes, assets, isometric components/systems, and rendering contracts. Remove `Silk.NET.OpenGL` and direct GL usage. Use `MiniEngine.*` namespaces. |
| `Graphics.OpenGL` | The only new project. References `Engine`; owns `Silk.NET.OpenGL`, unsafe GL code, handles, shaders, meshes, VAO/VBO state, textures, and `OpenGLRenderer`. Namespace: `MiniEngine.Graphics.OpenGL`. |
| `Game` | Game-specific entity/component setup and systems. References `Engine` only; remove direct Database, Silk.NET, renderer, and texture dependencies. |
| `Database` | Existing typed storage and terrain import. Continue returning neutral `MiniEngine.Assets.ImageData`; add the default asset-source implementation. |
| `AssetPipeline` | Keep image decoding and neutral `ImageData` output unchanged except namespace updates required by Engine. |
| `Bootstrap` | Composition root. References `Engine`, `Game`, `Graphics.OpenGL`, and `Database`; initializes the database and injects the selected asset source and OpenGL backend into the runtime. |

### Engine public API

All types below belong to `Engine`:

- `MiniEngine.Runtime.EngineHost`: owns the window and loop, creates the graphics backend after the native context loads, runs ECS update/render phases, and disposes resources.
- `IGame.Configure(GameContext context)`: replaces `Initialize(Renderer)`, `Update`, and `Render`. Game setup creates entities and registers systems; components receive no lifecycle callbacks.
- `GameContext`: exposes `World`, `IAssetSource`, and `TextureAssets`.
- `IAssetSource`: small generic `Get`/`TryGet` contract so developers can provide custom sources.
- `TextureAssets` and generation-safe `TextureAssetHandle`: load `ImageData` through `IAssetSource`, retain CPU-side assets, and lazily cache backend uploads.
- `IGraphicsBackendFactory.Create(GraphicsContext)` and `GraphicsContext`: backend integration contract containing only the graphics-procedure loader needed after window initialization.
- `IGraphicsBackend`: limited to currently required operations—begin/clear frame, create/destroy backend textures, draw a sprite command, end frame, and dispose.
- `BackendTextureHandle` and `SpriteDrawCommand`: opaque backend-neutral values used only between built-in rendering systems and the backend. Game components never contain backend handles.

All implementations below belong to `Graphics.OpenGL`:

- `OpenGLBackendFactory` creates `OpenGLRenderer` using `GL.GetApi` with the neutral procedure loader.
- `OpenGLRenderer` implements `IGraphicsBackend`.
- `OpenGLTexture`, `Shader`, `Mesh`, and VAO/VBO handling remain internal implementation types where practical.
- Every `GL` reference, OpenGL enum, numeric OpenGL handle, shader source, and unsafe GPU operation lives here.

## ECS, Scenes, and Isometric Model

All types below belong to `Engine`:

- `MiniEngine.ECS.World` owns one reusable ECS implementation, systems, component stores, entity generations, scenes, hierarchy data, and deferred structural commands.
- `Entity` is a stable index-plus-generation handle with fluent `Add`, `Remove`, `Has`, `Get`, `TryGet`, `SetParent`, `Detach`, and `Destroy` operations.
- Components are value types stored in per-type sparse/dense stores. Typed queries iterate the smallest relevant store and avoid reflection and per-frame allocations.
- Component values may be mutated directly by systems. Entity/component creation, removal, reparenting, and destruction during iteration go through a lightweight command buffer applied after the system phase.
- `IUpdateSystem` owns behavior; `World.AddSystem` preserves explicit registration order. Interface dispatch occurs once per system, never once per component.
- `Transform` contains data only: local `Vector3` position, Z rotation, and `Vector2` scale using `System.Numerics`.
- `WorldTransform` is system-produced, read-only to consumers, and recalculated only when the local transform or an ancestor version changes.
- Hierarchy uses compact ECS-managed adjacency/version data. Cycles, stale handles, and cross-scene parenting are rejected. Destroying a parent destroys its subtree; explicit detachment preserves world transform.
- `Scene` is a lightweight handle over the same `World`. Multiple scenes may be active additively; default queries process active-scene entities only. Unloading destroys the scene’s entities. Serialization, prefabs, inspectors, metadata, and separate per-scene worlds are deferred.
- `Sprite` stores a `TextureAssetHandle` plus only currently required size/render settings.
- `IsometricCamera` stores isometric projection settings, initially defaulting to the existing 32×16 tile footprint, 32-unit elevation step, and zoom 1.
- Built-in `HierarchySystem` propagates transforms before rendering.
- Built-in `IsometricRenderSystem` queries active entities with `Transform`/`WorldTransform` and `Sprite`, projects isometric coordinates, performs deterministic depth ordering with a reused buffer, and submits neutral `SpriteDrawCommand` values.
- Generic 2D remains possible later by adding another engine rendering system. Do not add 3D abstractions now.

Database adds `DatabaseAssetSource : IAssetSource`. Bootstrap populates the existing terrain database, injects this source, and injects `OpenGLBackendFactory`. Game loads `"tile_000"` through `GameContext.TextureAssets`, creates a scene, camera, and textured entity, and never receives a renderer.

## Incremental Implementation Milestones

1. **Document and stabilize**
   - Add `init.md` containing the agreed vision and rules.
   - Record that lightweight startup, old-device suitability, isometric quality, and simple component authoring are primary goals.
   - Diagnose the existing environment issue where `Engine.csproj` builds but `MiniEngine.sln` currently exits unsuccessfully without reported errors; establish a clean build baseline before refactoring.
   - Preserve all pre-existing user changes.

2. **Introduce ECS and scenes**
   - Implement stable entities, typed component stores, allocation-free queries, ordered systems, structural command buffering, hierarchy, dirty/versioned transforms, and additive scenes inside `Engine`.
   - Add focused invariant checks for stale handles, component operations, scene filtering, hierarchy cycles, detach behavior, subtree destruction, and scene unloading.
   - Leave the existing rendering route active until this milestone is complete and runnable.

3. **Perform the rendering and asset cutover**
   - Add `Graphics.OpenGL` to the solution and central package management.
   - Move all current renderer, texture, shader, mesh, and buffer behavior into it.
   - Add the minimal Engine rendering contracts, texture registry, and `IAssetSource`.
   - Implement `DatabaseAssetSource`, move database wiring to Bootstrap, and remove Game’s Database reference.
   - Migrate `IGame` to `Configure(GameContext)`, construct the demo through ECS components, and render through the built-in isometric system.
   - Delete the old `Engine.Graphics` types once all callers are migrated; add no aliases, wrappers, or obsolete compatibility APIs.

4. **Tighten boundaries and performance**
   - Remove `Silk.NET.OpenGL`, unsafe rendering code, and unused rendering package references from Engine and Game.
   - Ensure namespaces follow `MiniEngine.Runtime`, `.ECS`, `.Scenes`, `.Assets`, `.Rendering`, and `.Isometric`; use `.Graphics.OpenGL` only in the backend project.
   - Dispose cached GPU textures, shaders, meshes, buffers, and GL state during backend shutdown.
   - Update `init.md` with the final type map, lifecycle order, extension points, and completed milestones.

## Validation

- Build the complete solution after each milestone with zero errors; resolve the existing silent solution-build failure during milestone 1.
- Run Bootstrap after each milestone and verify window creation, update/render callbacks, terrain import, `"tile_000"` retrieval, GPU upload, and the textured quad/tile output.
- Verify dependency direction through project references and package inspection:
  - Game references Engine only.
  - Graphics.OpenGL references Engine.
  - Engine has no Graphics.OpenGL or Silk.NET.OpenGL reference.
  - Database has no OpenGL dependency.
  - Only Bootstrap names `OpenGLBackendFactory`.
- Search the repository to confirm `GL`, OpenGL enums, handles, shader sources, VAO/VBO code, and `unsafe` GPU operations exist only under `Graphics.OpenGL`.
- Exercise ECS invariants: stale entity rejection, fluent component access, filtered queries, deferred structural changes, hierarchy cycle rejection, dirty descendant propagation, subtree destruction, world-preserving detachment, cross-scene parenting rejection, additive activation, inactive-scene exclusion, and scene unloading.
- Confirm repeated sprites using one `TextureAssetHandle` produce one backend texture upload and that all GPU resources are released on shutdown.

## Assumptions

- This document is the implementation plan; `init.md` is created when implementation begins.
- Exactly one project is added: `Graphics.OpenGL`; no test, contracts, or additional rendering projects are introduced.
- Current visible behavior is the compatibility target. Existing public type shapes are not preserved.
- The database remains the provided default asset source, while `IAssetSource` permits developers to supply alternatives.
- No serialization, prefabs, editor/inspector support, reflection-driven registration, script callbacks, full archetype ECS, generic 2D feature work, or 3D architecture is included.
