# Importing images

Use `TextureAssets.Import` during game initialization:

```csharp
var grass = textureAssets.Import("/Assets/Terrain/Grass");
_tileTexture = grass["Terrain/Grass/tile_040"];
```

Paths are relative to the configured asset source. The optional `Assets/` or
`/Assets/` prefix is accepted. Folder and file names retain their filesystem case.

A directory import loads its immediate images. When there are no images in that
directory, it searches each subdirectory using the same rule. An empty directory
returns an empty dictionary. A missing directory throws `DirectoryNotFoundException`.

Use a trailing `/*` to import all images recursively, including children of
directories that already contain images:

```csharp
var textures = textureAssets.Import("Assets/*");
_tileTexture = textures["Terrain/Grass/tile_040"];
```

Non-image files are ignored. Supported extensions are `.png`, `.jpg`, `.jpeg`,
`.bmp`, `.gif`, `.tga`, `.psd`, `.hdr`, `.pic`, `.ppm`, and `.pgm`.
Invalid image data raises a decoding error. Directory enumeration skips symbolic
links to subdirectories. Other wildcard patterns are not supported.

Returned keys use `/` separators and are relative to the asset root. Lowercase
`.png` extensions are omitted so existing calls such as
`textureAssets.Load("Terrain/Grass/tile_040")` reuse imported handles. Other
extensions remain in the key. Repeated imports reuse the cached textures.

Import reads and decodes images into RAM. Each texture is uploaded to the GPU
when first used for drawing, then its GPU handle is reused.

## Validation

Build with `make build`. Run the dependency-free console regression checks with:

```sh
dotnet run --project tests/AssetPipeline.Tests/AssetPipeline.Tests.csproj
```

The checks cover folder fallback, recursive wildcards, empty and missing folders,
asset path prefixes, cached handles, decoded pixels, and deferred GPU uploads.
