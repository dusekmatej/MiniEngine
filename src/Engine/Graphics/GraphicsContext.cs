namespace MiniEngine.Graphics;

public readonly record struct GraphicsContext(
	Func<string, nint> GetProcAddress,
	int Width,
	int Height);