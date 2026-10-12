using MiniEngine.Core;

namespace MiniEngine.Graphics;

public interface IAssetSource
{
    string NormalizeName(string name) => name;

    ImageData LoadImage(string name);
}
