namespace MiniEngine.Rendering;

public readonly record struct GraphicsContext(Func<string, nint> GetProcAddress);