using MiniEngine.Core;
using MiniEngine.Graphics;

namespace MiniEngine.Database;

public sealed class DatabaseAssetSource : IAssetSource
{
    public ImageData LoadImage(string name)
        => Database.Get<ImageData>(name);
}