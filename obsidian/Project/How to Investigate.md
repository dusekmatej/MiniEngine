# How to Investigate MiniEngine

## Start Small

Read [[AI_CONTEXT]], then [[Project/Current State]]. Choose one code path from [[Project/Codebase Map]] instead of loading all project files.

## Match the Question to the Source

| Question | Start here |
| --- | --- |
| What runs first? | [[Project/Runtime Flow]] and `Bootstrap/Program.cs` |
| Why is an asset missing? | [[Project/Assets and Content]] and `TerrainImport.cs` |
| How is a texture drawn? | [[Knowledge/Graphics/Texture Lifecycle]] and `Renderer.cs` |
| How are systems found? | [[Knowledge/ECS/System Discovery]] and `SystemDiscovery.cs` |
| Is an ECS API usable today? | [[Project/Implementation and Plan]] and `World.cs` |
| Isometric coordinate issue? | [[Knowledge/Math/Isometric Projection]] and `IsometricProjection.cs` |

## Validate Before Concluding

The intended commands are `make restore`, `make build`, and `make run`. At the last vault inventory, `make build` failed without diagnostics; reproduce and investigate that baseline before presenting a build as known-good. See [[Development/Problems/Build baseline fails without diagnostics]].

## Keep Notes Useful

Capture durable intent, cause-and-effect, and decisions. Link source paths and existing notes. Do not paste long source files, terminal logs, or complete conversations into the vault.
