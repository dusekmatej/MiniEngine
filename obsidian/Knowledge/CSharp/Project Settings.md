# Project Settings

## Mental Model

Every current project targets `net10.0`, has implicit usings enabled, and enables nullable reference types. Package versions are centrally managed in `Directory.Packages.props`.

## How It Works

`Engine` and `OpenGL` allow unsafe blocks because the renderer passes matrix data to OpenGL. SILK.NET package versions and image/font library versions belong in the central package file, while individual project files declare the package references they need.

## MiniEngine Relevance

When adding a dependency, add or update the version centrally and add the project-level reference only where needed. Do not assume a package belongs in `Engine`: current dependencies are broader than the planned architecture, and future separation is still under discussion.

## Things I Confused

`ImplicitUsings` reduces repetitive imports but does not remove the need for correct project references. A type can compile in one project and be unavailable in another because project references define the assembly boundary.

## Related

[[Project/Codebase Map]] · [[Project/Implementation and Plan]]
