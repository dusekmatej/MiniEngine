
namespace Engine.Core;

public class ImageData
{
    public int Width { get; }
    public int Height { get; }

    public byte[] Pixels { get; }

    public ImageData(int width, int height, byte[] pixels)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
    }
}
