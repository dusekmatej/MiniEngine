# Core Concepts

This document contains the most important concepts and shared data types that developers will commonly encounter while creating a game with MiniEngine.

The goal is not to describe how the engine works internally, but to explain the objects that are exposed to game code and what responsibility each of them has.

---

## Entity

An `Entity` represents one object inside the game world.

An entity itself contains very little information. Its behavior and properties are created by attaching components to it.

For example:

```text
Entity
├── Transform
├── Velocity
├── Sprite
└── BoxCollider
```

Entities are managed by the `World`.

---

## World

`World` is the main container for ECS data.

It manages:

* entities
* components
* scenes
* ECS queries
* entity lifetime

Most interaction with entities and components eventually goes through the `World`.

---

## Scene

A `Scene` represents a group of entities inside the same `World`.

Scenes can be used to organize different parts of the game and control which groups of entities are currently active.

Examples could include:

```text
MainMenu
GameWorld
Interior
DebugScene
```

---

## GameContext

`GameContext` contains the main engine objects that are available when configuring a game.

Instead of exposing internal engine implementations, the engine provides the required game-facing functionality through this context.

Conceptually:

```csharp
public class GameContext
{
    public World World { get; }
    public IAssetSource Assets { get; }
    public TextureAssets Textures { get; }
}
```

This allows game code to create entities, access assets and configure the initial game state without depending on the engine internals.

---

## SystemContext

`SystemContext` contains data that is made available while engine behavior is being executed.

It provides a common place for shared runtime data that would otherwise need to be passed separately.

Conceptually:

```csharp
public class SystemContext
{
    public World World { get; }
    public float DeltaTime { get; }
}
```

The contents of `SystemContext` may expand as more shared runtime functionality is required.

---

## Transform

`Transform` describes where an entity exists in the world.

It commonly contains:

```text
Position
Rotation
Scale
```

Many other engine features can work with `Transform`, including rendering, collision detection, hierarchy and movement.

---

## WorldTransform

`WorldTransform` represents the final calculated transformation of an entity.

For entities without parents, this will normally be based directly on their `Transform`.

For child entities, the final transform can also include transformations inherited from their parent hierarchy.

Game code should usually modify `Transform` rather than `WorldTransform`.

---

## TextureAssetHandle

`TextureAssetHandle` is a lightweight reference to a texture managed by the engine.

Game code should store this handle instead of directly storing OpenGL textures or GPU identifiers.

For example:

```csharp
TextureAssetHandle grassTexture;
```

The engine is then responsible for loading, caching and uploading the actual texture when required.

---

## TextureAssets

`TextureAssets` provides access to textures managed by MiniEngine.

It connects asset data with the rendering backend while keeping game code independent from OpenGL or another graphics API.

Conceptually:

```csharp
var texture = context.Textures.Load("tile_000");
```

The returned value can then be stored inside components using a `TextureAssetHandle`.

---

## IAssetSource

`IAssetSource` represents a source from which game assets can be loaded.

The game does not need to know whether an asset originally came from:

* a database
* files
* generated content
* another custom source

MiniEngine communicates with these sources through the same abstraction.

---

## ImageData

`ImageData` is a backend-neutral representation of image data.

It contains the information required to create a texture without exposing OpenGL or another graphics API.

Conceptually, it can contain values such as:

```text
Width
Height
Pixel Data
```

This allows image loading and graphics rendering to remain separate responsibilities.

---

# Responsibility Overview

The important concepts can be viewed roughly like this:

```text
Game
 │
 ├── GameContext
 │      ├── World
 │      ├── IAssetSource
 │      └── TextureAssets
 │
 └── World
        ├── Scenes
        └── Entities
              └── Components

Runtime execution
 │
 └── SystemContext
        └── World

Assets
 │
 ├── IAssetSource
 ├── ImageData
 └── TextureAssetHandle
```

Each type should have a clearly separated responsibility and game code should not need access to backend-specific engine implementations.
