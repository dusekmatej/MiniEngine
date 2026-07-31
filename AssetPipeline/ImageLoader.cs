using Engine.Core;
using StbImageSharp;

namespace AssetPipeline;

public static class ImageLoader
{
    public static ImageData Load(string path)
    {
        using var stream = File.OpenRead(path);

        ImageResult image =
            ImageResult.FromStream(
                stream,
                ColorComponents.RedGreenBlueAlpha);


        return new ImageData(
            image.Width,
            image.Height,
            image.Data);
    }
}