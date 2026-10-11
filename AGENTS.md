# Repository Guidelines

## Project Structure & Module Organization

MiniEngine is a C# game engine organized in `MiniEngine.sln`:

- `src/Engine/`: ECS components, entities, systems, graphics interfaces, and world management.
- `src/OpenGL/`: rendering backend, shaders, meshes, and textures.
- `src/AssetPipeline/`: image decoding and filesystem sources; `src/Database/`: generic structured-data storage.
- `Game/`: sample game implementation; `Game/Bootstrap/`: executable entry point.
- `Game/Assets/`: fonts and image assets, copied recursively to build and publish output.
- `docs/`: architecture and subsystem documentation; see `docs/AssetPipeline.md` for image loading.

Keep reusable engine behavior in `Engine/`, backend implementation in `OpenGL/`, and game-specific behavior in `Game/`.

## Build, Test, and Development Commands

Install the .NET 10 SDK. Run these commands from the repository root:

- `make restore` — restore NuGet dependencies (`dotnet restore`).
- `make build` — build the solution (`dotnet build`).
- `make run` — launch the sample game through `Game/Bootstrap/Bootstrap.csproj`.
- `make clean` — remove build outputs (`dotnet clean`).
- `make format` — apply C# formatting (`dotnet format`); review the diff before committing.

Running the game requires a graphical environment with OpenGL support.

## Coding Style & Naming Conventions

Follow existing C# conventions: four-space indentation, braces on separate lines, and file-scoped namespaces. Use PascalCase for types, methods, and properties; camelCase for parameters and locals; and `_camelCase` for private fields. Prefix interfaces with `I`, such as `IGraphicsBackend`. Name files after their primary type and preserve established namespace organization.

Prefer readable steps over compressed expressions. Use braces for control-flow bodies and descriptive local variables for nontrivial path, validation, and resource-lifecycle logic. Keep expression-bodied members for obvious getters and simple forwarding; avoid dense one-liners, nested ternaries, and null-forgiving operators that hide assumptions. Use a straightforward loop when it is clearer than LINQ, and keep refactors focused on the affected code.

Nullable reference types and implicit usings are enabled. Manage dependency versions centrally in `Directory.Packages.props`.

## Testing Guidelines

Run `dotnet test MiniEngine.sln` for the xUnit asset pipeline tests in `tests/AssetPipeline.Tests`. Validate changes with `make build` and exercise affected behavior through `make run`. For rendering changes, inspect the output visually. Record validation steps and any failures in the pull request. No coverage threshold is established.

Remove temporary verification files and stop any processes you start after validation. Preserve committed automated tests.

## Commit & Pull Request Guidelines

History uses short descriptive subjects such as `Added EntityManager` and occasional `FIX:` prefixes; no strict convention is established. Write clear, focused commit subjects describing the change.

Pull requests should explain the problem, implementation, and validation performed. Link relevant issues, include screenshots for visible rendering changes, and update `docs/` when architecture or public behavior changes.
