# Entity handles carry generations

## Decision

The current implementation represents an entity as a readonly value handle containing `Id` and `Generation`. `EntityManager` validates both values for liveness and increments the generation on destruction.

## Why

The code makes stale handles distinguishable from a later entity that reuses the same numeric ID. No separate written rationale was found, so this note records observable behavior rather than adding a new justification.

## Alternatives

No alternatives are recorded in the repository.

## Consequences

Code that manages entity-associated data must account for reuse of IDs. A raw integer is insufficient as a durable entity reference across destruction and creation.

## Related

[[Knowledge/ECS/Entity Handles]] · [[Knowledge/ECS/Component Stores]]
