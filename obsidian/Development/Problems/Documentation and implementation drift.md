# Documentation and implementation drift

## Problem

Some repository documentation describes desired or incomplete APIs as though they are available today.

## Symptoms

`docs/Concepts.md` and `docs/ComponentSystem.md` discuss `World` queries, scenes, `GameContext`, texture assets, hierarchy behavior, and systems accessing a world. Current `World.cs` only stores an `EntityManager`; current `SystemContext` exposes only delta time; `IGame` receives graphics and texture manager directly.

## Cause

The project is actively evolving. Documentation was recently generated and explicitly marked for revision in Git history, while `plan.md` contains a proposed refactor that has not been fully implemented.

## Solution

Use source code as the current implementation authority. Use docs and `plan.md` to understand intent, then label a note as current, proposed, or unknown. Update the affected docs when an API becomes real rather than copying aspirational examples into implementation work.

## What I Learned

Docs can be valuable design context without being an API reference. Keeping the distinction explicit saves agents from proposing changes against nonexistent APIs.

## Related

[[Project/Implementation and Plan]] · [[Project/Architecture]] · [[Project/Current State]]
