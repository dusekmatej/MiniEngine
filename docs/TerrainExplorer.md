# Terrain explorer

Run `make run`. The window starts at 800×600 and supports resizing, maximizing and restoring.

- WASD or arrow keys pan in screen directions.
- Hold Shift to move three times faster.
- The mouse wheel zooms between 0.25× and 4×. Movement speed follows zoom.
- Home returns to the starting camera position and zoom.

`Game/Environment/TerrainGeneration.cs` configures deterministic terrain with broad plains and less common mountain ranges. Ground noise selects deep water below 34, shallow water at 34–40, dirt shores at 40–43 and grass above 43. Most land stays flat at `PlainsHeight` (one block). A separate low-frequency noise field selects mountain regions; only values above `MountainThreshold` (0.60) add relief. Smooth blending and ridged detail produce slopes and peaks up to `MaximumHeight` (18 blocks). Mountain surfaces become earth from height 6 and stone from height 9. Water remains at height 0.

Grass, dirt and stone use existing images in `Game/Assets/Terrain/isometric_assets`. Grass variants form coherent patches and use a muted tint. Water uses `Terrain/Water/tile_092` and `tile_104`. Source images remain unchanged, and there are no decoration overlays.

Each engine `Chunk` stores a `Heights` array alongside tile IDs. `GetHeight` samples world coordinates, including negative positions, so chunk boundaries and unloaded neighbors agree. `MainWorld` streams 16×16 chunks around the camera and regenerates them deterministically on return. A small margin reduces churn, and the maximum elevation expands the streamed area to keep raised surfaces visible near screen edges. This sample does not persist edits to unloaded chunks. The HUD shows resident, generated and unloaded chunk counts.

The game uses `IsometricPreset.Blocks`: footprint width 0.30, footprint height 0.15 and elevation step 0.15. Every height change is a whole block. `Drawing.DrawWorld` projects a diamond top and two vertical faces with exact shared corners. It samples the top and side regions of the existing images using `Graphics2D.DrawTexturedQuad`; this avoids inheriting the shallow side thickness or different projection of the source sprites. Only exposed side faces are drawn, with distinct lighting on the left and right. Neighbor heights determine exposure. Water is a flat diamond surface at sea level. Both world and standalone chunk drawing use this renderer, with a global diagonal order across chunks.

`DrawTexturedQuad` adds optional affine position and UV transforms to texture commands; ordinary sprites retain their existing behavior. The original `IsometricPreset.Default` remains available for the separate sprite demos.

To tune the landscape, increase `MountainThreshold` for fewer mountain ranges, change `PlainsHeight` for the baseline land elevation, or change `MaximumHeight` for taller or shorter peaks. Keep the block preset's elevation step equal to its footprint height for full block proportions.

The engine forwards framebuffer resize events to `IGraphicsBackend.Resize` and updates `Graphics2D` viewport dimensions. The OpenGL backend uses those dimensions on every frame. Framebuffer dimensions also cover displays where logical window dimensions differ from physical pixels. Custom graphics backends must implement `Resize(int width, int height)`.

For graphical validation, inspect broad flat grass areas, mountain slopes, cliff faces and shorelines. Pan across chunk boundaries, zoom out, maximize and restore, resize to wide and tall shapes, and return with Home. Terrain should reach the view edges and match on return while the loaded chunk count stays bounded by the view.
