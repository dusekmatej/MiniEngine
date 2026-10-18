using MiniEngine.AssetPipeline;
using MiniEngine.Core;
using MiniEngine.Graphics;

string assetRoot = Path.Combine(Path.GetTempPath(), "MiniEngine-Import-" + Guid.NewGuid());

try
{
    string grassDirectory = Path.Combine(assetRoot, "Terrain", "Grass");
    string nestedDirectory = Path.Combine(grassDirectory, "Nested");
    Directory.CreateDirectory(nestedDirectory);
    Directory.CreateDirectory(Path.Combine(assetRoot, "Empty"));

    byte[] pixels = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aX1sAAAAASUVORK5CYII=");
    File.WriteAllBytes(Path.Combine(grassDirectory, "tile.png"), pixels);
    File.WriteAllBytes(Path.Combine(nestedDirectory, "other.png"), pixels);
    File.WriteAllText(Path.Combine(assetRoot, "notes.txt"), "Not an image");

    var source = new FileAssetSource(assetRoot);
    var graphics = new RecordingGraphicsBackend();
    var textures = new TextureAssets(graphics, source);

    IReadOnlyDictionary<string, TextureAssetHandle> grass = textures.Import("/Assets/Terrain/Grass");
    Check(grass.Count == 1, "A directory containing images should stop at that level.");
    Check(grass.ContainsKey("Terrain/Grass/tile"), "Names should be relative to Assets and omit .png.");
    Check(graphics.UploadCount == 0, "Import should leave GPU uploads lazy.");

    IReadOnlyDictionary<string, TextureAssetHandle> terrain = textures.Import("Assets/Terrain");
    Check(terrain.Count == 1, "A folder without images should search its children.");
    Check(terrain["Terrain/Grass/tile"] == grass["Terrain/Grass/tile"], "Repeated imports should reuse handles.");

    IReadOnlyDictionary<string, TextureAssetHandle> all = textures.Import("Assets/*");
    Check(all.Count == 2, "A wildcard should include nested images even when the parent has images.");
    Check(all.ContainsKey("Terrain/Grass/Nested/other"), "Nested images should retain their folder paths.");
    Check(textures.Load("Terrain/Grass/tile") == grass["Terrain/Grass/tile"], "Load should reuse imported handles.");
    Check(textures.Import("/Assets/Empty").Count == 0, "An empty folder should return an empty result.");
    Check(textures.Import("Assets\\Terrain\\Grass\\*").Count == 2, "Backslashes should work in import paths.");

    TextureAssetHandle tile = grass["Terrain/Grass/tile"];
    textures.GetBackendHandle(tile);
    textures.GetBackendHandle(tile);
    Check(graphics.UploadCount == 1, "Repeated drawing should upload a texture only once.");
    Check(source.LoadImage("/Assets/Terrain/Grass/tile").Width == 1, "Assets-prefixed image paths should resolve.");

    ExpectException<DirectoryNotFoundException>(() => textures.Import("Assets/Missing"));
    ExpectException<ArgumentException>(() => textures.Import("Assets/../Outside"));
    ExpectException<ArgumentException>(() => textures.Import("Assets/Ter*rain"));

    Console.WriteLine("Asset import checks passed.");
}
finally
{
    Directory.Delete(assetRoot, true);
}

static void Check(bool condition, string message)
{
    if (!condition)
        throw new Exception(message);
}

static void ExpectException<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new Exception($"Expected {typeof(TException).Name}.");
}

sealed class RecordingGraphicsBackend : IGraphicsBackend
{
    public int UploadCount { get; private set; }

    public BackendTextureHandle CreateTexture(ImageData image)
    {
        if (image.Width != 1 || image.Height != 1 || image.Pixels.Length != 4)
            throw new Exception("Expected decoded RGBA pixels.");

        UploadCount++;
        return default;
    }

    public void Resize(int width, int height) { }

    public void BeginFrame() { }
    public void Clear() { }
    public void EndFrame() { }
    public void DestroyTexture(BackendTextureHandle texture) { }
    public void DrawTexture(TextureDrawCommand command) { }
    public void DrawRectangle(RectangleDrawCommand command) { }
    public void DrawTriangle(TriangleDrawCommand command) { }
    public void DrawCircle(CircleDrawCommand command) { }
    public void DrawLine(LineDrawCommand command) { }
    public void DrawText(TextDrawCommand command) { }
}
