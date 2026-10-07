# Entity Handles

## Mental Model

An `Entity` is a small identifier, not an object that owns components. The current handle contains an integer `Id` and a `Generation` number.

## How It Works

`EntityManager.Create` either reuses an ID from a free-ID stack or appends a new generation entry. `Destroy` first checks whether the handle is alive, increments that ID's generation, and returns the ID to the stack. `IsAlive` validates both the ID range and generation equality.

An old handle becomes stale after its entity is destroyed, even if the numeric ID is later reused. Systems that introduce component stores, world operations, or queries should preserve that check rather than use the ID alone as identity.

## MiniEngine Relevance

The current `World` receives an `EntityManager`, but it does not yet expose create or destroy operations. Any integration should establish exactly where liveness checks happen before coupling stores to entity IDs.

## Things I Confused

An entity's ID is reusable; the `(Id, Generation)` pair identifies a particular lifetime. A component store indexed only by ID therefore needs a liveness policy when entities are destroyed or IDs are reused.

## Related

[[Knowledge/ECS/Component Stores]] · [[Project/Decisions/Entity handles carry generations]]
