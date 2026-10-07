# Implementation and Plan

This note prevents a common orientation error: treating `plan.md` or old documentation as the current codebase.

## Current Implementation

- The solution contains `OpenGL`, not `Graphics.OpenGL`.
- `Engine` still references SILK.NET windowing, input, math, and OpenGL packages.
- `Game` references both `Database` and `OpenGL` projects and receives `IGraphicsBackend` plus `TextureManager` through `IGame.Initialize`.
- `World` currently only stores an `EntityManager`; it does not expose queries, systems, scenes, or component operations.
- `ComponentStore<T>` currently implements `Add`, `Remove`, and `Has`; it lacks public retrieval and enumeration.
- Rendering is invoked directly from `Game.Render` using graphics APIs and draw commands.

## Proposal in `plan.md`

The plan calls for an isometric-first, data-oriented ECS. It proposes a backend-neutral `Engine`, an OpenGL-only implementation project, a composition root that wires assets and rendering, ECS scenes and hierarchy, neutral rendering commands, and game code that creates ECS data rather than talks to a renderer.

It also proposes generation-safe texture assets, a command-buffer approach to ECS structural changes, system registration, hierarchy propagation, and an isometric render system. These are requirements for the proposed refactor, not evidence that the behavior exists now.

## How to Use This Distinction

For a bug or small change, start from the current source and preserve working behavior. For a task explicitly adopting part of the refactor, identify the exact proposed milestone, compare it with current dependencies, and update a decision note once accepted.

Do not copy API examples from `docs/Concepts.md` or `docs/ComponentSystem.md` into production code without checking that the types and methods exist. Several examples describe planned `World`, `Scene`, `GameContext`, and query APIs.

## Related

[[Project/Roadmap]] · [[Development/Problems/Documentation and implementation drift]] · [[Project/Architecture]]
