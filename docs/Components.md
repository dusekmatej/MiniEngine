# Components

## Basic idea
In MiniEngine, **components** are pure data containers. They hold state — nothing else. All behaviour lives in systems, which read and write component data each frame.

If you want an entity to have a position, you attach a `TransformComponent`. If you want it to move, you attach a `VelocityComponent`. The movement logic itself lives in a system.

You can read more about how systems consume components in [Systems Core](SystemsCore.md).

Components are stored in a `ComponentStore<T>`, which uses a sparse set internally for fast lookup and iteration. You never touch the store directly — systems receive access to component data through `SystemContext`.

---

## Most important parts

`IComponent`
Marker interface that every component must implement. The constraint `where T : struct, IComponent` on `ComponentStore<T>` enforces this at compile time.

`ComponentStore<T>`
The internal storage for a single component type. Backed by a sparse set. Supports `Add`, `Remove`, `Has`, `Get`, and `TryGet`.

`TransformComponent` / `WorldTransformComponent`
The two most fundamental components — local space and resolved world space.

---

## All components

### Identity & Metadata

---

#### NameComponent
Stores a human-readable label for an entity. Useful during debugging and editor tooling.

```csharp
public string Name;
```

---

#### TagComponent
Attaches a short category string to an entity. Systems can filter entities by tag without inspecting multiple components.

```csharp
public string Tag;
```

Example tags: `"Player"`, `"Enemy"`, `"Pickup"`

---

#### LifetimeComponent
Tracks how long an entity has remaining before it should be destroyed. `TotalTime` lets you compute how far through its life the entity is — useful for effects like fading out.

```csharp
public float TotalTime;
public float RemainingTime;
```

```csharp
float progress = 1f - (lifetime.RemainingTime / lifetime.TotalTime); // 0 → 1 as entity ages
```

---

#### DisabledComponent
Signals that an entity should be skipped by systems. The entity stays in the world but is treated as inactive.

```csharp
public bool IsDisabled;
```

> If `IsDisabled` is true, systems should early-out for that entity.

---

### Transform & Hierarchy

---

#### TransformComponent
The local spatial state of an entity — position, rotation, and scale relative to its parent (or the world if it has none).

```csharp
public float X;
public float Y;
public float Rotation;
public Vector2 Scale;
```

- `X`, `Y` — local position.
- `Rotation` — local rotation in degrees.
- `Scale` — local size multiplier.

---

#### WorldTransformComponent
The resolved world-space transform, computed from an entity's local transform and its full parent chain. This is what rendering and physics systems should read.

```csharp
public Vector3 Position;
public float Rotation;
public Vector2 Scale;
```

`Position` uses `Vector3` to support elevation (the Z axis) for isometric layouts.

> Do not write to this component directly. It is computed by the hierarchy system from `TransformComponent` and `ParentComponent`.

---

#### ParentComponent
Links an entity to a parent, making it part of a transform hierarchy. The child's `WorldTransformComponent` is resolved relative to its parent's world transform.

```csharp
public int ParentEntityIndex;
```

```
Player (TransformComponent)
└── Sword (TransformComponent + ParentComponent → Player)
```

---

### Movement & Physics

---

#### VelocityComponent
How fast the entity is moving, per frame.

```csharp
public float X;
public float Y;
public float DeltaTime;
```

- `X`, `Y` — movement speed along each axis.
- `DeltaTime` — elapsed time since the last frame, used to scale movement correctly.

---

#### AccelerationComponent
The rate at which velocity changes each frame. Keep this separate from `VelocityComponent` — forces accumulate here and are applied to velocity by a physics system.

```csharp
public float X;
public float Y;
```

---

#### AngularVelocityComponent
How fast the entity is rotating.

```csharp
public float AngularVelocity;
```

Value is in degrees per second.

---

#### RigidBodyComponent
Marks the entity as a physics body and defines its physical properties.

```csharp
public float Mass;
public bool UseGravity;
public bool IsStatic;
```

- `IsStatic` — if true, the entity does not move regardless of forces applied (e.g. walls, floors).
- `UseGravity` — whether the global gravity affects this body.

---

#### GravityComponent
Scales the strength of gravity for a specific entity, independent of the global gravity value.

```csharp
public float GravityScale;
```

`1.0` = normal gravity, `0.0` = no gravity, `2.0` = twice as heavy.

---

#### FrictionComponent
Slows the entity down over time when no force is being applied.

```csharp
public float Friction;
```

Higher values = faster deceleration.

---

#### MovementConstrainComponent
Locks movement along one or both axes.

```csharp
public bool ConstrainX;
public bool ConstrainY;
```

Useful for entities that should only move in one direction, such as a platform or rail-locked enemy.

---

#### RotationConstraintComponent
Prevents physics from rotating the entity.

```csharp
public bool LockRotation;
```

Attach this to characters that should stay upright regardless of collisions.

---

### Collision

---

#### BoxColliderComponent
A rectangular collision shape. Offset fields shift the collider relative to the entity's position.

```csharp
public float Width;
public float Height;
public float OffsetX;
public float OffsetY;
```

---

#### CircleColliderComponent
A circular collision shape.

```csharp
public float Radius;
public float OffsetX;
public float OffsetY;
```

---

#### CollisionLayerComponent
The layer this entity belongs to, expressed as a bitmask. Used together with `CollisionMaskComponent` to filter which entities interact with each other.

```csharp
public int Layer;
```

---

#### CollisionMaskComponent
Which layers this entity should collide with, expressed as a bitmask. A collision only occurs if the other entity's `CollisionLayerComponent` matches this mask.

```csharp
public int Mask;
```

```
// Entity A is on layer 1, and collides with layer 2
CollisionLayer = 0b0001
CollisionMask  = 0b0010

// Entity B is on layer 2, and collides with layer 1
CollisionLayer = 0b0010
CollisionMask  = 0b0001
// → A and B will collide with each other
```

---

#### TriggerComponent
Marker component. Entities with this component detect overlaps but do not produce a physical collision response.

```csharp
// No fields — presence alone is the signal
```

Use for checkpoints, damage zones, pickup areas, etc.

---

### Rendering

---

#### SpriteComponent
The display size of the entity's sprite in world units.

```csharp
public Vector2 Size;
```

---

#### VisibilityComponent
Controls whether the entity is rendered at all.

```csharp
public bool IsVisible;
```

The rendering system should skip entities where `IsVisible` is false.

---

#### OpacityComponent
How transparent the entity's sprite is.

```csharp
public float Opacity;
```

`1.0` = fully visible, `0.0` = fully transparent.

---

#### TintComponent
A colour multiplier applied on top of the sprite. Uses RGBA.

```csharp
public Vector4 Tint;
```

`(1, 1, 1, 1)` = no tint. Useful for damage flashes, team colours, or environment effects.

---

#### RenderLayerComponent
Controls draw order. Higher layer values are drawn on top.

```csharp
public int Layer;
```

---

### Camera

---

#### CameraComponent
Marks an entity as a camera and defines how it views the world. Only one camera should be active at a time.

```csharp
public float Zoom;
public bool IsActive;
```

- `Zoom` — `1.0` = normal view, above `1.0` zooms in, below zooms out.
- `IsActive` — the rendering system uses whichever camera has this set to true.
