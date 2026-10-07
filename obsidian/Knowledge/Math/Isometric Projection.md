# Isometric Projection

## Mental Model

The current projection turns a three-dimensional grid-like world position into a two-dimensional screen position using an isometric preset and a camera.

## How It Works

`IsometricProjection.WorldToScreen` first subtracts `Camera.Position`. It then computes:

```text
screenX = (relativeX - relativeY) * footprintWidth / 2
screenY = -(relativeX + relativeY) * footprintHeight / 2
screenY += relativeZ * elevationStep
```

The method multiplies both screen coordinates by `Camera.Zoom` and adds a caller-supplied screen origin. `GetSpriteSize` applies that same zoom to the preset sprite width and height. `Camera.Zoom` must be greater than zero.

`IsometricPreset.Default` has a 0.30 × 0.30 sprite, a 0.30 × 0.15 footprint, and a 0.075 elevation step. These are current display parameters, not universal isometric constants.

## MiniEngine Relevance

Use this class for tasks about camera movement, tile placement, coordinate errors, or a future rendering-system integration. Verify draw ordering separately: projecting positions decides where items appear, but it does not by itself sort overlapping sprites.

## Things I Confused

Increasing Z adds to `screenY` in this current formula. Whether that corresponds to visually moving an object up or down depends on the renderer's coordinate convention and the model matrix; test it in the running sample.

## Related

[[Knowledge/Graphics/Isometric Tile Sample]] · [[Project/Assets and Content]]
