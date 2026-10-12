using MiniEngine.AssetPipeline;
using MiniEngine.Graphics;
using Xunit;

namespace MiniEngine.Tests;

public sealed class FileAssetSourceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "MiniEngine-assets-" + Guid.NewGuid().ToString("N"));
    private readonly FileAssetSource _source;

    public FileAssetSourceTests()
    {
        Directory.CreateDirectory(Path.Combine(_root, "Terrain", "Grass"));
        File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures", "tile_022.png"), Path.Combine(_root, "Terrain", "Grass", "tile.png"));
        _source = new FileAssetSource(_root);
    }

    [Theory]
    [InlineData("Terrain/Grass/tile")]
    [InlineData("Terrain/Grass/tile.png")]
    [InlineData("./Terrain//Grass\\tile.png")]
    public void LoadsNestedRgbaImage(string name)
    {
        var image = _source.LoadImage(name);
        Assert.Equal(32, image.Width);
        Assert.Equal(32, image.Height);
        Assert.Equal(image.Width * image.Height * 4, image.Pixels.Length);
        Assert.Contains((byte)0, image.Pixels.Where((_, index) => index % 4 == 3));
        Assert.Contains((byte)255, image.Pixels.Where((_, index) => index % 4 == 3));
    }

    [Fact]
    public void EquivalentNamesShareLogicalHandle()
    {
        using var textures = new TextureAssets(new FakeGraphicsBackend(), _source);
        var handle = textures.Load("Terrain/Grass/tile");
        Assert.Equal(handle, textures.Load("./Terrain//Grass\\tile.png"));
        Assert.Equal(1, textures.RegisteredCount);
        Assert.Equal(0, textures.UploadedCount);
    }

    [Fact]
    public void ExtensionResolutionPrefersPng()
    {
        File.WriteAllText(Path.Combine(_root, "Terrain", "Grass", "tile.jpg"), "invalid jpeg");
        Assert.EndsWith("tile.png", _source.NormalizeName("Terrain/Grass/tile"), StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(_source.LoadImage("Terrain/Grass/tile"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("../outside")]
    [InlineData("Terrain/../../outside.png")]
    [InlineData("/outside.png")]
    [InlineData("C:\\outside.png")]
    [InlineData("\\\\server\\outside.png")]
    [InlineData("./")]
    [InlineData("bad\0.png")]
    public void RejectsInvalidPaths(string name)
        => Assert.ThrowsAny<ArgumentException>(() => _source.LoadImage(name));

    [Fact]
    public void MissingImagesAreUsefulErrors()
    {
        var error = Assert.Throws<FileNotFoundException>(() => _source.LoadImage("missing"));
        Assert.Contains("missing", error.Message);
        Assert.Contains(_root, error.Message);
    }

    [Fact]
    public void UnsupportedExtensionIsRejected()
        => Assert.Throws<NotSupportedException>(() => _source.LoadImage("image.tga"));

    [Fact]
    public void InvalidImageIsDecodedOnlyOnFirstUse()
    {
        File.WriteAllText(Path.Combine(_root, "broken.png"), "not an image");
        using var textures = new TextureAssets(new FakeGraphicsBackend(), _source);
        var handle = textures.Load("broken");
        var error = Assert.Throws<InvalidDataException>(() => textures.GetBackendHandle(handle));
        Assert.Contains("broken.png", error.Message);
        Assert.Equal(0, textures.UploadedCount);
    }

    [Fact]
    public void RejectsDirectoryAndFileSymlinksIncludingReplacementAfterRegistration()
    {
        string outside = Path.Combine(Path.GetTempPath(), "MiniEngine-outside-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outside);
        try
        {
            File.Copy(Path.Combine(_root, "Terrain", "Grass", "tile.png"), Path.Combine(outside, "tile.png"));
            Directory.CreateSymbolicLink(Path.Combine(_root, "escape"), outside);
            File.CreateSymbolicLink(Path.Combine(_root, "escape.png"), Path.Combine(outside, "tile.png"));
            Assert.Throws<UnauthorizedAccessException>(() => _source.LoadImage("escape/tile"));
            Assert.Throws<UnauthorizedAccessException>(() => _source.LoadImage("escape.png"));
            using var textures = new TextureAssets(new FakeGraphicsBackend(), _source);
            var handle = textures.Load("Terrain/Grass/tile");
            File.Delete(Path.Combine(_root, "Terrain", "Grass", "tile.png"));
            File.CreateSymbolicLink(Path.Combine(_root, "Terrain", "Grass", "tile.png"), Path.Combine(outside, "tile.png"));
            Assert.Throws<UnauthorizedAccessException>(() => textures.Preload(handle));
        }
        finally
        {
            Directory.Delete(outside, true);
        }
    }

    [Fact]
    public void GifLoadsFirstFrameAsRgba()
    {
        File.WriteAllBytes(Path.Combine(_root, "single.gif"), Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"));
        var image = _source.LoadImage("single");
        Assert.Equal(1, image.Width);
        Assert.Equal(1, image.Height);
        Assert.Equal(4, image.Pixels.Length);
    }

    [Theory]
    [InlineData("red.jpg", "red.jpg")]
    [InlineData("red.jpg", "red.jpeg")]
    [InlineData("red.bmp", "red.bmp")]
    [InlineData("animated.gif", "animated.gif")]
    public void SupportedFormatsDecodeCorrectRgba(string fixture, string asset)
    {
        File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures", fixture), Path.Combine(_root, asset));
        var image = _source.LoadImage(Path.GetFileNameWithoutExtension(asset));
        Assert.Equal(2, image.Width);
        Assert.Equal(1, image.Height);
        Assert.Equal(8, image.Pixels.Length);
        Assert.InRange(image.Pixels[0], (byte)250, (byte)255);
        Assert.InRange(image.Pixels[1], (byte)0, (byte)5);
        Assert.Equal(255, image.Pixels[3]);
    }

    public void Dispose() => Directory.Delete(_root, true);
}
