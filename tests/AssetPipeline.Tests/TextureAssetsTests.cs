using MiniEngine.Core;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Fonts;
using Xunit;

namespace MiniEngine.Tests;

public sealed class TextureAssetsTests
{
    private sealed class CountingSource : IAssetSource
    {
        public int DecodeCount { get; private set; }
        public ImageData LoadImage(string name)
        {
            DecodeCount++;
            return new ImageData(1, 1, [10, 20, 30, 40]);
        }
    }

    [Fact]
    public void LazyLoadCachesSingleDecodeAndUpload()
    {
        var source = new CountingSource();
        var backend = new FakeGraphicsBackend();
        using var textures = new TextureAssets(backend, source);
        var handle = textures.Load("grass");
        Assert.Equal(handle, textures.Load("grass"));
        Assert.Equal(0, source.DecodeCount);
        Assert.Equal(0, backend.CreateCount);
        var gpu = textures.GetBackendHandle(handle);
        Assert.Equal(gpu, textures.GetBackendHandle(handle));
        Assert.Equal(1, source.DecodeCount);
        Assert.Equal(1, backend.CreateCount);
        Assert.Equal(0, textures.RetainedImageCount);
    }

    [Fact]
    public void UnloadPreservesHandleAndReconstructsGpuResource()
    {
        var source = new CountingSource();
        var backend = new FakeGraphicsBackend();
        using var textures = new TextureAssets(backend, source);
        var handle = textures.Load("grass");
        var first = textures.GetBackendHandle(handle);
        textures.Unload(handle);
        textures.Unload(handle);
        Assert.Equal(1, backend.DestroyCount);
        Assert.Equal(0, textures.UploadedCount);
        Assert.Equal(handle, textures.Load("grass"));
        Assert.NotEqual(first, textures.GetBackendHandle(handle));
        Assert.Equal(2, source.DecodeCount);
        Assert.Equal(2, backend.CreateCount);
    }

    [Fact]
    public void RetentionPolicyAvoidsDecodeAfterEvictionButReloadRefreshesSource()
    {
        var source = new CountingSource();
        using var textures = new TextureAssets(new FakeGraphicsBackend(), source, retainSourceImages: true);
        var handle = textures.Load("grass");
        textures.Preload(handle);
        Assert.Equal(1, textures.RetainedImageCount);
        textures.Unload(handle);
        textures.Preload(handle);
        Assert.Equal(1, source.DecodeCount);
        textures.Reload(handle);
        Assert.Equal(2, source.DecodeCount);
        Assert.Equal(1, textures.RetainedImageCount);
    }

    [Fact]
    public void GeneratedImagesSurviveEvictionAndReload()
    {
        var backend = new FakeGraphicsBackend();
        using var textures = new TextureAssets(backend);
        var image = new ImageData(1, 1, [255, 255, 255, 128]);
        var handle = textures.Load("glyph", image);
        Assert.Equal(handle, textures.Load("glyph", image));
        textures.Preload(handle);
        textures.Unload(handle);
        textures.Preload(handle);
        textures.Reload(handle);
        Assert.Same(image, backend.LastImage);
        Assert.Equal(3, backend.CreateCount);
        Assert.Equal(1, textures.RetainedImageCount);
    }

    [Fact]
    public void RemoveInvalidatesHandleWithoutReusingSlot()
    {
        var backend = new FakeGraphicsBackend();
        using var textures = new TextureAssets(backend, new CountingSource());
        var old = textures.Load("grass");
        textures.Preload(old);
        textures.Remove(old);
        var replacement = textures.Load("grass");
        Assert.NotEqual(old, replacement);
        Assert.Throws<InvalidOperationException>(() => textures.GetBackendHandle(old));
        Assert.Throws<InvalidOperationException>(() => textures.GetBackendHandle(replacement with { Generation = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => textures.GetBackendHandle(new TextureAssetHandle(-1, 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => textures.GetBackendHandle(new TextureAssetHandle(100, 1)));
        Assert.Equal(1, backend.DestroyCount);
    }

    [Fact]
    public void CleanupDestroysAllResourcesExactlyOnce()
    {
        var backend = new FakeGraphicsBackend();
        var textures = new TextureAssets(backend, new CountingSource());
        var a = textures.Load("a");
        var b = textures.Load("b");
        textures.Load("unused");
        textures.Preload([a, b]);
        textures.Dispose();
        textures.Dispose();
        Assert.Equal(2, backend.DestroyCount);
        Assert.Empty(backend.LiveTextures);
        Assert.Equal(0, textures.RegisteredCount);
        Assert.Equal(0, textures.UploadedCount);
        Assert.Equal(0, textures.RetainedImageCount);
        Assert.Throws<ObjectDisposedException>(() => textures.Load("a"));
    }

    [Fact]
    public void FailedUploadCanBeRetried()
    {
        var backend = new FakeGraphicsBackend { FailUpload = true };
        using var textures = new TextureAssets(backend, new CountingSource());
        var handle = textures.Load("a");
        Assert.Throws<InvalidOperationException>(() => textures.Preload(handle));
        Assert.Equal(0, textures.UploadedCount);
        backend.FailUpload = false;
        textures.Preload(handle);
        Assert.Equal(1, textures.UploadedCount);
    }

    [Fact]
    public void WrongThreadIsRejectedBeforeBackendAccess()
    {
        var backend = new FakeGraphicsBackend();
        using var textures = new TextureAssets(backend, new CountingSource());
        var handle = textures.Load("a");
        Exception? error = null;
        var thread = new Thread(() => error = Record.Exception(() => textures.Preload(handle)));
        thread.Start();
        thread.Join();
        Assert.IsType<InvalidOperationException>(error);
        Assert.Equal(0, backend.CreateCount);
    }

    [Theory]
    [InlineData(0, 1, 4)]
    [InlineData(-1, 1, 4)]
    [InlineData(1, 0, 4)]
    [InlineData(2, 2, 4)]
    [InlineData(int.MaxValue, int.MaxValue, 0)]
    [InlineData(1073741824, 1073741824, 0)]
    public void InvalidRgbaDimensionsOrBufferAreRejected(int width, int height, int length)
        => Assert.ThrowsAny<ArgumentException>(() => new ImageData(width, height, new byte[length]));

    [Fact]
    public void MissingSourceAndNullPixelsAreRejected()
    {
        using var textures = new TextureAssets(new FakeGraphicsBackend());
        Assert.Throws<InvalidOperationException>(() => textures.Load("a"));
        Assert.Throws<ArgumentNullException>(() => new ImageData(1, 1, null!));
    }

    [Fact]
    public void DrawingSpritesAndFontsReusesTextureResources()
    {
        var backend = new FakeGraphicsBackend();
        using var textures = new TextureAssets(backend, new CountingSource());
        var fonts = new FontManager();
        var font = fonts.AddFont("test", Path.Combine(AppContext.BaseDirectory, "Fixtures", "font.ttf"));
        var drawing = new Graphics2D(backend, textures, fonts);
        var sprite = textures.Load("sprite");
        for (int i = 0; i < 2; i++)
        {
            drawing.DrawSprite(sprite, 0, 0, 1, 1, EngineColor.White);
            drawing.DrawText(font, "AA", 0, 0, 0.1f, EngineColor.White);
            drawing.Flush();
        }
        Assert.Equal(2, backend.CreateCount);
        Assert.Equal(2, backend.TextureDrawCount);
        Assert.Equal(4, backend.TextDrawCount);
    }

    [Fact]
    public void GenericDatabaseStorageStillWorks()
    {
        var storage = new MiniEngine.Database.DatabaseStorage<int>();
        storage.Import("test", 42);
        Assert.Equal(42, storage.Get("test"));
        Assert.True(storage.TryGet("test", out int value));
        Assert.Equal(42, value);
    }
}
