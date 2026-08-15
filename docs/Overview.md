# Architecture Overview

This section contains a high-level overview of the MiniEngine architecture.

## Architecture Diagram

![MiniEngine Architecture - not yet created](./images/architecture-overview.png)

## Engine Architecture

MiniEngine is divided into smaller parts with clearly separated responsibilities.

Detailed documentation for each part can be opened below.

### Systems

Systems contain the behavior of the ECS architecture and process components or other engine data.

[Open Systems Documentation →](./SystemsCore.md)

---

## Concepts

Short explanations of important MiniEngine concepts.

### [`Update()`](./Systems/SystemsCore.md#update)

Called during the engine update phase and used by `IUpdateSystem` implementations.

### [`SystemContext`](./Systems/SystemsCore.md#systemcontext)

Provides systems with the engine data they need during execution.
