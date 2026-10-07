# ECS integration is incomplete

## Problem

Entity and component-storage pieces exist, but they are not yet a usable ECS surface for game or system code.

## Symptoms

`EntityManager` manages generation-aware entities. `ComponentStore<T>` and `ComponentStoreManager` exist. `World` stores an entity manager but exposes no operations, and `SystemContext` carries only delta time. Current systems cannot query component combinations through public runtime APIs.

## Cause

The repository is in the component-store integration phase. Existing comments in `ComponentStore<T>` also call out cleanup requirements when entities or worlds are destroyed.

## Solution

Undecided. Establish the build baseline first, then define the smallest required world operations and store invariants before extending queries or rendering systems. Preserve generation safety and fix sparse/dense swap-remove bookkeeping as part of that work.

## What I Learned

Having component structs and a store type is not the same as having an ECS API. Entity lifecycle, storage invariants, access patterns, and system context must meet at an intentional world boundary.

## Related

[[Knowledge/ECS/Entity Handles]] · [[Knowledge/ECS/Component Stores]] · [[Project/Roadmap]]
