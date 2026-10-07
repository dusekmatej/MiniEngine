# Isometric Tile Sample

## Mental Model

The current sample renders an isometric tile platform directly from `Game.Game`; it is not yet an ECS render system.

## How It Works

`Game` creates a `TileMap` sized 9×9 and uses `IsometricPreset.Default`. `DrawPlatform` iterates diagonal depths from zero through `(GridSize - 1) * 2`. For each depth it derives valid `gridX` and `gridY` pairs, skips values other than tile ID zero, and computes:

```text
screenX = originX + (gridX - gridY) * footprintWidth / 2
screenY = originY - (gridX + gridY) * footprintHeight / 2
```

It then submits one texture command using the preset sprite dimensions. Diagonal traversal gives a simple, deterministic back-to-front-like ordering for this flat platform.

## MiniEngine Relevance

`IsometricProjection` provides a more reusable `WorldToScreen` function that includes camera position, zoom, and Z elevation. The sample's current direct calculation does not call it. This is evidence of an evolving path, not a reason to delete either version without an agreed change.

## Things I Confused

The map stores integer tile IDs, whereas `TileDefinitionRegistry` produces `TileId` values for registered definitions. The sample currently treats the map value `0` as the rendered tile and does not connect the map to the registry.

## Related

[[Knowledge/Math/Isometric Projection]] · [[Project/Implementation and Plan]]
