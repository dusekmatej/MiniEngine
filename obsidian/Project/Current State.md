# Current State

## Current Focus

ECS and rendering architecture are under active development. Component storage is being integrated into the engine while the sample game's isometric terrain rendering remains the practical integration target.

## Working Systems

- `Bootstrap/Program.cs` creates `Game`, constructs an OpenGL backend factory, and runs `Engine`.
- `Engine` wraps a SILK.NET window and forwards load, update, and render events.
- The OpenGL backend creates textures and currently draws texture, rectangle, and text commands.
- The sample game imports terrain image files, loads `tile_000`, draws a 9×9 isometric tile platform, and initializes font support.
- Entity generations, reflection-based system discovery, and base component types are present.
- `TextureManager` caches named images and creates the backend texture only when it is first requested.

## In Progress

- Integrating component storage with the broader ECS/world model. The current store has `Add`, `Remove`, and `Has`; it does not yet expose retrieval, queries, or world integration.
- Moving component files from the misspelled historical `Engine/Componets/` path into `Engine/Components/` is visible in the working tree. Treat that migration as unfinished until the build baseline is restored.
- Evolving rendering and asset boundaries. Confirm behavior in source code before relying on existing docs or `plan.md`.

## Known Problems

- `make build` currently exits with `Build FAILED`, zero warnings, and zero errors. See [[Development/Problems/Build baseline fails without diagnostics]].
- Current source and some documentation are out of sync. See [[Development/Problems/Documentation and implementation drift]].

## Next

Diagnose the build baseline before making broader architectural changes. Then confirm the next ECS or rendering milestone, record its intent or decision, and validate it by building and running the sample.
