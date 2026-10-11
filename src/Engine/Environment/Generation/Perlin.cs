using System.Numerics;

namespace MiniEngine.Environment;

public class Perlin
{
    private readonly int[] _perm;

    private static readonly Vector2[] Gradients =
    {
        new( 1f,      0f     ),
        new(-1f,      0f     ),
        new( 0f,      1f     ),
        new( 0f,     -1f     ),
        new( 0.7071f,  0.7071f),
        new(-0.7071f,  0.7071f),
        new( 0.7071f, -0.7071f),
        new(-0.7071f, -0.7071f),
    };

    public Perlin(int seed)
    {
        _perm = new int[512];
        int[] base256 = GeneratePermutation(seed);
        for (int i = 0; i < 512; i++)
            _perm[i] = base256[i % 256];
    }

    public float Sample(float x, float y)
    {
        int xi = (int)MathF.Floor(x) & 255;
        int yi = (int)MathF.Floor(y) & 255;

        float xf = x - MathF.Floor(x);
        float yf = y - MathF.Floor(y);

        int A  = (_perm[xi]     + yi) & 255;
        int B  = (_perm[xi + 1] + yi) & 255;
        int AA = _perm[A];
        int AB = _perm[(A + 1) & 255];
        int BA = _perm[B];
        int BB = _perm[(B + 1) & 255];

        float dotAA = Dot(Gradients[AA & 7], xf,     yf    );
        float dotBA = Dot(Gradients[BA & 7], xf - 1, yf    );
        float dotAB = Dot(Gradients[AB & 7], xf,     yf - 1);
        float dotBB = Dot(Gradients[BB & 7], xf - 1, yf - 1);

        float u = Fade(xf);
        float v = Fade(yf);

        float result = Lerp(
            Lerp(dotAA, dotBA, u),
            Lerp(dotAB, dotBB, u),
            v
        );

        return Math.Clamp((result + 0.7f) / 1.4f, 0f, 1f);
    }

    // Octaves = více vrstev šumu přes sebe → přirozenější terén
    public float SampleOctaves(float x, float y, int octaves, float persistence, float lacunarity)
    {
        float value     = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxValue  = 0f;

        for (int i = 0; i < octaves; i++)
        {
            value     += Sample(x * frequency, y * frequency) * amplitude;
            maxValue  += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return value / maxValue;
    }

    private static float Dot(Vector2 g, float x, float y) => g.X * x + g.Y * y;
    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private static float Lerp(float a, float b, float t) => a + t * (b - a);

    private static int[] GeneratePermutation(int seed)
    {
        Random rand = new Random(seed);
        int[] p = new int[256];

        for (int i = 0; i < 256; i++)
            p[i] = i;

        for (int i = 255; i > 0; i--)
        {
            int j    = rand.Next(i + 1);
            int temp = p[i];
            p[i]     = p[j];
            p[j]     = temp;
        }

        return p;
    }
}