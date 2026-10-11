
namespace MiniEngine.Core;

public class ImageData
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }

    public ImageData(int width, int height, byte[] pixels)
    {
        ArgumentNullException.ThrowIfNull(pixels);
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Image dimensions must be positive.");
        long pixelCount = (long)width * height;
        if (pixelCount > int.MaxValue / 4 || pixelCount * 4 != pixels.LongLength)
            throw new ArgumentException("Image pixels must contain exactly width * height * 4 RGBA bytes.", nameof(pixels));

        Width = width;
        Height = height;
        Pixels = pixels;
    }
}
