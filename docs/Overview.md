# Architecture Overview

This section contains a high-level overview of the MiniEngine architecture.

## Architecture Diagram

![MiniEngine Architecture - not yet created](./images/architecture-overview.png)

## Engine Architecture

MiniEngine is divided into smaller parts with clearly separated responsibilities.

Detailed documentation for each part can be opened below.

### Components

Components are data containers that store the state of entities in the ECS architecture. Learn how to create and use components.

[Open Components Documentation →](./ComponentSystem.md)

### Systems

Systems contain the behavior of the ECS architecture and process components or other engine data.

[Open Systems Documentation →](./SystemsCore.md)

---

### Rendering API

[Open Rendering API Documentation →](./rendering-api.md)

---

## Concepts

Short explanations of important MiniEngine concepts.

### [Component structs](./ComponentSystem.md#what-are-components)

Components are plain data structs. Component stores accept any struct type.

### [`ComponentStore<T>`](./ComponentSystem.md#component-storage-architecture)

Efficient sparse-set storage for components. Enables fast lookup and cache-friendly iteration.

### [`Update()`](./SystemsCore.md#update)

Called during the engine update phase and used by `IUpdateSystem` implementations.

### [`SystemContext`](./SystemsCore.md#systemcontext)

Provides systems with the engine data they need during execution.
