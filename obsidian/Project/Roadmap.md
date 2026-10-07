# Roadmap

## Confirmed Direction

The README names ECS, automatic system discovery, rendering abstraction, asset management, scene and entity management, and isometric rendering as active development areas. Recent history also shows work on the entity manager, component storage, text rendering, map systems, and rendering APIs.

## Current Immediate Work

Component storage has been added but is not yet exposed through `World` or system context. First establish a repeatable solution build and runnable sample baseline, then integrate ECS behavior in small, verified steps. See [[Project/Current State]] and [[Development/Problems/Build baseline fails without diagnostics]].

## Proposed Refactor Plan

`plan.md` describes a larger isometric, data-oriented ECS refactor. It proposes a backend-neutral engine, a separate `Graphics.OpenGL` project, game code that avoids renderer and database dependencies, scenes, hierarchy, asset sources, and built-in render systems. This is a proposal, not current implementation.

Use [[Project/Implementation and Plan]] to compare a task against that proposal. Do not implement a proposed item merely because it appears in the plan; confirm the active milestone first.

## Milestone Discipline

For each accepted milestone, add its goal, constraints, validation command, and related decision here. Keep the demo runnable after each meaningful step. Record postponed ideas in [[Ideas/]] rather than expanding the current task silently.
