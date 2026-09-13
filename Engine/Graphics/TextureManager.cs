namespace MiniEngine.Graphics;

public class TextureManager : TextureAssets
{
    public TextureManager(IGraphicsBackend graphics)
        : base(graphics)
    {
    }

    public TextureManager(
        IGraphicsBackend graphics,
        IAssetSource assetSource)
        : base(graphics, assetSource)
    {
    }
}