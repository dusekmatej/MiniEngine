# Systems are discovered automatically

## Decision

The current system model discovers non-abstract `ISystem` classes by reflection rather than requiring manual registration for every system.

## Why

`docs/SystemsCore.md` states that the architecture is designed so users do not need to manually register new systems. `SystemDiscovery` and related factory code implement that approach.

## Alternatives

Manual registration and explicit system ordering are not recorded as adopted alternatives. `plan.md` proposes explicit `World.AddSystem` registration for a future refactor, so do not treat that proposal as current behavior.

## Consequences

Systems currently need to be instantiable through a parameterless constructor. Discovery order and priority scheduling should be verified rather than assumed.

## Related

[[Knowledge/ECS/System Discovery]] · [[Project/Runtime Flow]] · [[Project/Implementation and Plan]]
