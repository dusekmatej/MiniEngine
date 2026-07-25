using Silk.NET.OpenGL;
using Engine.Graphics;

namespace Engine.Core;

public class Engine
{
    private Window _window; // Don't mistake the IWindow with Window class
    private GameLoop _loop;
    private IGame _game;
    private Renderer? _renderer;

    public Engine(IGame game)
    {
        _game = game;
        _game.Initialize();
        _window = new Window();

        var time = new Time();
        _loop = new GameLoop(time);

        _window.Load += () =>
        {
            var gl = GL.GetApi(_window.NativeWindow);
            _renderer = new Renderer(gl);
        };

        _window.Update += _ =>
        {
            _loop.Tick();
            _game.Update(time.DeltaTime);
        };

        _window.Render += _ =>
        {
            _renderer?.Clear();

            if (_renderer is not null)
            {
                foreach (var tile in _loop.TilesToDraw)
                {
                    _renderer.DrawRectangle(tile.X, tile.Y, tile.Width, tile.Height);
                }
            }

            _game.Render();
        };
    }

    public void Run()
    {
        _window.Run();
    }
}