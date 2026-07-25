using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Engine.Core;

public class Window
{
    private readonly IWindow _window;

    public IWindow NativeWindow => _window;

    public event Action? Load;
    public event Action<double>? Update;
    public event Action<double>? Render;

    public Window()
    {
        var windowOptions = WindowOptions.Default;

        windowOptions.Size = new Vector2D<int>(800, 600);
        windowOptions.Title = "Engine";

        windowOptions.VSync = false;
        windowOptions.FramesPerSecond = 26000;
        
        _window = Silk.NET.Windowing.Window.Create(windowOptions);
        
        // Window events
        _window.Load += () =>
        {
            Load?.Invoke();
        };

        _window.Update += delta =>
        {
            Update?.Invoke(delta);
        };

        _window.Render += delta =>
        {
            Render?.Invoke(delta);
        };
    }

    public void Run() 
        => _window.Run();

    public void Close()
    {
        _window.Close();
    }
}