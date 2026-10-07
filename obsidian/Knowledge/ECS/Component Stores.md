# Component Stores

## Mental Model

Each component type has its own storage. The current `ComponentStore<T>` is a sparse-set-shaped structure: a sparse array maps an entity ID to a dense position, while dense arrays hold entities and component values.

## How It Works

`Add` extends `_sparse` until the entity ID fits, appends the entity and component to the dense arrays, then stores the dense index in `_sparse[entity.Id]`. `Has` rejects negative or out-of-range IDs and considers `-1` absent. `Remove` marks the sparse entry absent and replaces the removed dense component with the final dense component before removing the final item.

The implementation is incomplete. It has no `Get`, `TryGet`, enumeration, duplicate-add protection, or update of the moved entity's sparse index during removal. It also does not currently remove the matching dense entity entry. These are implementation gaps visible in source, not just missing documentation.

## MiniEngine Relevance

`ComponentStoreManager` keeps a dictionary from component `Type` to `IComponentStore` and creates stores on demand. It is internal and has no current connection to `World`. This explains why component structs exist but systems cannot yet query them through a world API.

## Things I Confused

Dense storage is not automatically correct just because the arrays are dense. Swap-remove requires updating every index that points to the moved final item, and entity destruction must clean every affected store.

## Related

[[Knowledge/ECS/Entity Handles]] · [[Project/Current State]] · [[Development/Problems/ECS integration is incomplete]]
