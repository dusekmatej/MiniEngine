# Renderer Backend

## Mental Model

`OpenGL/Core/Renderer.cs` is the current adapter between engine-level draw commands and SILK.NET OpenGL calls. `OpenGL/Core/BackendFactory.cs` obtains a `GL` instance after the window has a native context and returns this renderer as `IGraphicsBackend`.

## How It Works

The renderer creates one quad mesh and three shader programs during construction: textured, solid color, and text. It enables alpha blending with source-alpha and one-minus-source-alpha blending. `Clear` uses a dark blue-gray clear color.

`CreateTexture` turns an `ImageData` into an OpenGL texture and stores it in a dictionary keyed by a `BackendTextureHandle`. `DrawTexture`, `DrawRectangle`, and `DrawText` select a shader, create a model matrix from command position and size, set uniforms, bind the needed texture or mesh, then issue a triangle-fan draw for four vertices.

## MiniEngine Relevance

The renderer does not own a scene, camera, batching system, or ECS query. The sample game calculates screen positions and submits commands itself. This is the current behavior even though `plan.md` proposes engine-owned isometric rendering later.

## Things I Confused

`TextureAssetHandle` is an engine-level handle managed by `TextureManager`; `BackendTextureHandle` identifies the renderer's texture dictionary. They are not interchangeable.

## Related

[[Knowledge/Graphics/Texture Lifecycle]] · [[Project/Runtime Flow]] · [[Project/Implementation and Plan]]
