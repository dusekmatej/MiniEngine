namespace MiniEngine.Graphics;

public readonly record struct EngineColor
{
    public readonly float R { get; }
    public readonly float G { get; }
    public readonly float B { get; }
    public readonly float A { get; }

    private EngineColor(float r, float g, float b, float a)
    {
        R = Math.Clamp(r, 0f, 1f);
        G = Math.Clamp(g, 0f, 1f);
        B = Math.Clamp(b, 0f, 1f);
        A = Math.Clamp(a, 0f, 1f);
    }

    public static EngineColor FromRgb(int r, int g, int b)
        => FromRgba(r, g, b, 255);

    public static EngineColor FromRgba(int r, int g, int b, int a)
    {
        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);
        a = Math.Clamp(a, 0, 255);

        return new EngineColor(
            r / 255,
            g / 255,
            b / 255,
            a / 255
        );
    }

    public static EngineColor FromNormalized(float r, float g, float b, float a = 1f)
    {
        r = Math.Clamp(r, 0f, 1f);
        g = Math.Clamp(g, 0f, 1f);
        b = Math.Clamp(b, 0f, 1f);
        a = Math.Clamp(a, 0f, 1f);

        return new EngineColor(r, g, b, a);
    }

    public static EngineColor Transparent => FromRgba(0, 0, 0, 0);
    public static EngineColor Black => FromRgb(0, 0, 0);
    public static EngineColor White => FromRgb(255, 255, 255);
    public static EngineColor Red => FromRgb(255, 0, 0);
    public static EngineColor Green => FromRgb(0, 255, 0);
    public static EngineColor Blue => FromRgb(0, 0, 255);
    public static EngineColor Yellow => FromRgb(255, 255, 0);
    public static EngineColor Cyan => FromRgb(0, 255, 255);
    public static EngineColor Magenta => FromRgb(255, 0, 255);
}