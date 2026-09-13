namespace MiniEngine.Graphics;

public readonly record struct LineDrawCommand
{
    public float StartX { get; }
    public float StartY { get; }
    public float EndX { get; }
    public float EndY { get; }
    public float Thickness { get; }
    public EngineColor Color { get; }
    public int Layer { get; }

    public LineDrawCommand(
        float startX,
        float startY,
        float endX,
        float endY,
        float thickness,
        EngineColor color,
        int layer = 0)
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
        Thickness = thickness;
        Color = color;
        Layer = layer;
    }
}