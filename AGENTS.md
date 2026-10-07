# Repository Guidelines

## Project Structure & Module Organization

MiniEngine is a C# game engine organized in `MiniEngine.sln`:

- `Engine/`: ECS components, entities, systems, graphics interfaces, and world management.
- `OpenGL/`: rendering backend, shaders, meshes, and textures.
- `AssetPipeline/`: image loading; `Database/`: asset access and terrain resources.
- `Game/`: sample game implementation; `Bootstrap/`: executable entry point.
- `Game/Assets/`, `Database/Terrain/`, and `AllTextures/`: fonts and image assets.
- `docs/`: architecture and subsystem documentation; start with `docs/Overview.md`.

Keep reusable engine behavior in `Engine/`, backend implementation in `OpenGL/`, and game-specific behavior in `Game/`.

## Build, Test, and Development Commands

Install the .NET 10 SDK. Run these commands from the repository root:

- `make restore` — restore NuGet dependencies (`dotnet restore`).
- `make build` — build the solution (`dotnet build`).
- `make run` — launch the sample game through `Bootstrap/Bootstrap.csproj`.
- `make clean` — remove build outputs (`dotnet clean`).
- `make format` — apply C# formatting (`dotnet format`); review the diff before committing.

Running the game requires a graphical environment with OpenGL support.

## Coding Style & Naming Conventions

Follow existing C# conventions: four-space indentation, braces on separate lines, and file-scoped namespaces. Use PascalCase for types, methods, and properties; camelCase for parameters and locals; and `_camelCase` for private fields. Prefix interfaces with `I`, such as `IGraphicsBackend`. Name files after their primary type and preserve established namespace organization.

Nullable reference types and implicit usings are enabled. Manage dependency versions centrally in `Directory.Packages.props`.

## Testing Guidelines

The repository currently has no automated test project, test framework, or coverage threshold. Validate changes with `make build` and exercise affected behavior through `make run`. For rendering changes, inspect the output visually. Record validation steps and any failures in the pull request. If adding tests, document the chosen framework and execution command.

## Commit & Pull Request Guidelines

History uses short descriptive subjects such as `Added EntityManager` and occasional `FIX:` prefixes; no strict convention is established. Write clear, focused commit subjects describing the change.

Pull requests should explain the problem, implementation, and validation performed. Link relevant issues, include screenshots for visible rendering changes, and update `docs/` when architecture or public behavior changes.
