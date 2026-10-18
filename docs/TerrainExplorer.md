# Terrain explorer

Run `make run`. The window starts at 800×600 and supports resizing, maximizing and restoring.

- WASD or arrow keys pan in screen directions.
- Hold Shift to move three times faster.
- The mouse wheel zooms between 0.25× and 4×. Movement speed follows zoom.
- Home returns to the starting camera position and zoom.

`Game/Environment/TerrainGeneration.cs` configures the sample terrain using the engine's seeded Perlin generator. The bands concentrate near the middle of the noise distribution: deep water below 39, shallow water at 39–44, dirt shores at 44–47, grass at 47–59, earth at 59–63 and stone above 63. Texture variants and sparse flower/rock overlays use stable world-coordinate hashes. Flower patches also use a separate vegetation noise field. Assets come from `Game/Assets/Terrain` and decorations are drawn over complete ground tiles.

`MainWorld` generates 16×16 chunks around the view, including negative coordinates. It removes distant chunks and regenerates them deterministically on return. A small margin outside the screen reduces churn. This sample does not persist edits to unloaded chunks. The HUD shows resident, generated and unloaded chunk counts.

`Drawing.DrawWorld` rejects sprites outside the view and draws the remaining tiles in a global diagonal order across chunk boundaries. The horizontal projection uses the current framebuffer aspect ratio to keep the isometric tiles proportional.

The engine forwards framebuffer resize events to `IGraphicsBackend.Resize` and updates `Graphics2D` viewport dimensions. The OpenGL backend uses those dimensions on every frame. Framebuffer dimensions also cover displays where logical window dimensions differ from physical pixels. Custom graphics backends must implement `Resize(int width, int height)`.

For graphical validation, pan across chunk boundaries in both directions, zoom out, maximize and restore, resize to both wide and tall shapes, and return with Home. Terrain should reach all view edges and match on return while the loaded chunk count stays bounded by the view.
