# System Discovery

## Mental Model

The current runtime discovers systems by reflection so a user does not manually register every `ISystem` class.

## How It Works

`AssemblyDiscovery` collects loaded assemblies and references of the entry assembly. `SystemDiscovery` inspects loadable types and accepts classes that implement `ISystem` and are not abstract. `SystemFactory` creates each one through `Activator.CreateInstance`, so a parameterless constructor is required.

`PriorityAttribute` can provide an integer value. The named levels range from `Highest` (0) to `Lowest` (1500), with `Normal` at 500. The factory stores the priority in `SystemInfo`.

## MiniEngine Relevance

`Engine` discovers systems during construction and invokes discovered `IUpdateSystem` instances on each update. The current loop iterates the discovered list without sorting it, so priority is recorded but not enforced by that code path. A discovery-order dependency is fragile and should not become an implicit contract.

## Things I Confused

Automatic discovery and priority ordering are separate features. Adding a priority attribute does not establish execution order unless the runtime sorts or otherwise schedules the systems.

## Related

[[Project/Runtime Flow]] · [[Project/Decisions/Systems are discovered automatically]]
