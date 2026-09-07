# Component System

## Basic Idea

MiniEngine uses an **Entity-Component-System (ECS)** architecture. Components are the **data containers** that hold the state of entities. Unlike systems which contain behavior, components are value types (structs) that store only data.

Think of it this way:
- **Entity**: A unique identifier that represents a game object (a player, enemy, tile, etc.)
- **Component**: Pure data attached to an entity (position, velocity, color, etc.)
- **System**: Logic that reads and modifies components

## What Are Components?

Components are simple C# structs that implement the `IComponent` marker interface. They contain **only data** — no logic or methods.

### Basic Component Example

```csharp
using MiniEngine.Components.Core;

namespace MiniEngine.Components;

public struct VelocityComponent : IComponent
{
    public float X;
    public float Y;
    public float DeltaTime;
}
```

That's it! A component is just a struct with fields. The `IComponent` interface tells the engine that this struct is a component.

### Common Built-in Components

MiniEngine comes with several pre-made components:

- `TransformComponent` — Position (X, Y), rotation, and scale
- `VelocityComponent` — Movement velocity and delta time
- `NameComponent` — A name for the entity
- `TagComponent` — Tags for categorization
- `LifetimeComponent` — For entities that should be destroyed after a duration
- `DisabledComponent` — To deactivate entities without destroying them

## How to Create a New Component

1. **Create a struct** that implements `IComponent`
2. **Add only data fields** — no methods or properties
3. **Use simple types**: `float`, `int`, `Vector2`, `string`, etc.

### Example: Health Component

```csharp
using MiniEngine.Components.Core;

namespace MiniEngine.Components;

public struct HealthComponent : IComponent
{
    public float CurrentHealth;
    public float MaxHealth;
    public bool IsDead;
}
```

### Example: Sprite Component

```csharp
using MiniEngine.Components.Core;
using MiniEngine.Graphics;

namespace MiniEngine.Components;

public struct SpriteComponent : IComponent
{
    public TextureAssetHandle TextureHandle;
    public int Width;
    public int Height;
    public bool IsVisible;
}
```

## Component Storage Architecture

Components are stored in a **sparse-set data structure**, which is efficient for game performance.

### How Storage Works (Conceptual)

Each component type gets its own `ComponentStore<T>`:

- **Sparse array**: Maps entity IDs to component locations (used for quick lookups)
- **Dense array**: Stores actual component data (cache-friendly for iteration)

This means:
- **Fast lookup**: Getting a component is O(1)
- **Fast iteration**: All components of a type are stored contiguously (cache-friendly)
- **Easy removal**: Swap-remove keeps everything compact

### Under the Hood

You don't need to interact with storage directly, but here's how it works:

```csharp
internal sealed class ComponentStore<T> : IComponentStorage
    where T : struct, IComponent
{
    private int[] _sparse;              // Maps entity index → dense index
    private int[] _denseEntities;       // Entity IDs in order
    private T[] _denseComponents;       // Component data in order
    
    public ref T Get(int entityIndex);           // Get mutable reference
    public bool Has(int entityIndex);            // Check if entity has this component
    public void Add(int entityIndex, T component);  // Add component to entity
    public bool Remove(int entityIndex);         // Remove component from entity
    public bool TryGet(int entityIndex, out T component);  // Safe get
}
```

**Key insight**: The `Get()` method returns a `ref`, which means you can modify the component data directly without creating copies.

## Components and Entities

Every component is attached to an **entity**. An entity is identified by an integer index.

In the ECS flow:
1. **Create an entity** (gets a unique integer ID)
2. **Add components** to that entity
3. **Systems query** for entities with specific component combinations
4. **Systems modify** component data

Example entity operations (future API):

```csharp
// Create an entity
Entity player = world.CreateEntity();

// Add components
player.Add(new TransformComponent { X = 10, Y = 20 });
player.Add(new VelocityComponent { X = 2, Y = 0 });
player.Add(new HealthComponent { CurrentHealth = 100, MaxHealth = 100 });

// Query and modify
if (player.Has<VelocityComponent>())
{
    ref var vel = ref player.Get<VelocityComponent>();
    vel.X += vel.DeltaTime * 5;  // Update position
}
```

## How Systems Access Components

Systems receive a `SystemContext` and can access components through the world. Here's how it works:

```csharp
using MiniEngine.Systems.Core;
using static MiniEngine.Systems.Core.PriorityLevel;

[Priority(High)]
public class MovementSystem : IUpdateSystem
{
    public void Update(SystemContext context)
    {
        // In the future, SystemContext will provide access to the World
        // For now, components are being integrated into the system
        
        // Once integrated, it will look like:
        // var world = context.World;
        
        // // Iterate all entities with Transform and Velocity
        // foreach (var entity in world.Query<TransformComponent, VelocityComponent>())
        // {
        //     ref var transform = ref entity.Get<TransformComponent>();
        //     ref var velocity = ref entity.Get<VelocityComponent>();
        //     
        //     // Update position based on velocity
        //     transform.X += velocity.X * context.DeltaTime;
        //     transform.Y += velocity.Y * context.DeltaTime;
        // }
    }
}
```

## Component Queries

The storage system enables **efficient queries**:

```csharp
// Get all entities with a specific component
var allMoving = world.Query<VelocityComponent>();

// Get entities with a combination of components
// (only entities that have both components)
var allRenderable = world.Query<TransformComponent, SpriteComponent>();

// Iteration is cache-friendly since components are stored contiguously
foreach (var entity in allRenderable)
{
    var transform = entity.Get<TransformComponent>();
    var sprite = entity.Get<SpriteComponent>();
    // Process...
}
```

Queries are **zero-allocation** — no lists are created, just iteration over the underlying arrays.

## Component Design Best Practices

### ✅ DO:
- Keep components focused on one aspect of data
- Use simple value types (struct)
- Make components pure data with no logic
- Add only fields needed for this component's concept

### ❌ DON'T:
- Add methods or complex logic to components
- Hold references to other entities (use IDs if needed)
- Add constructor logic beyond initialization
- Create huge components with every possible property

### Good Component Example

```csharp
public struct CollisionComponent : IComponent
{
    public float Radius;
    public CollisionLayer Layer;
    public bool IsColliding;
}
```

### Poor Component Example

```csharp
// ❌ Too much logic, mixed concerns
public struct BadComponent : IComponent
{
    public float Health;
    public float Mana;
    public float Stamina;
    public Texture Sprite;
    public AudioClip Sound;
    
    public void TakeDamage(float amount)  // Don't do this!
    {
        Health -= amount;
    }
}
```

## Component Lifecycle

Components follow these states:

1. **Created**: A component is created with initial values
2. **Attached**: Added to an entity through `Add()`
3. **Modified**: Systems update the component data
4. **Removed**: Removed from an entity through `Remove()`
5. **Destroyed**: Destroyed when the entity is destroyed

```csharp
// Lifecycle example
var entity = world.CreateEntity();

// 1. Create & Attach
entity.Add(new HealthComponent { CurrentHealth = 100, MaxHealth = 100 });

// 2. Check & Modify
if (entity.Has<HealthComponent>())
{
    ref var health = ref entity.Get<HealthComponent>();
    health.CurrentHealth -= 10;  // Modified
}

// 3. Remove
entity.Remove<HealthComponent>();

// 4. Destroyed (when entity is destroyed)
entity.Destroy();
```

## Current Integration Status

The component system is being integrated into the full ECS architecture. Here's what's ready now:

✅ **Complete**:
- Component struct definition with `IComponent` marker
- Sparse-set storage with efficient access and iteration
- Component queries across stored types
- Built-in components (`TransformComponent`, `VelocityComponent`, etc.)

🔄 **In Progress** (Milestone 2):
- `World` entity management
- `Entity` handle and fluent API
- `SystemContext` providing world access
- Hierarchy system
- Structural command buffering (deferred add/remove/destroy)

## See Also

[Systems Core →](./SystemsCore.md) — How systems use components  
[Architecture Overview →](./Overview.md) — How components fit into the engine

