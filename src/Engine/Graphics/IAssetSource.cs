using MiniEngine.Core;

namespace MiniEngine.Graphics;

public interface IAssetSource
{
    ImageData LoadImage(string name);

    IReadOnlyList<string> GetImageNames(string path);
}
