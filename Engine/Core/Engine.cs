using Silk.NET.OpenGL;
using MiniEngine.Graphics;
using MiniEngine.Systems.Core;

namespace MiniEngine.Core;

public class Engine
{
    private Window _window; // Don't mistake the IWindow with Window class
    private GameLoop _loop;
    private IGame _game;
    private IGraphicsBackendFactory _graphicsFactory;
    private IGraphicsBackend _graphics;
    private TextureManager? _textureManager;

    // TESTING FIELD FOR SYSTEMS
    private readonly List<SystemInfo> _systems; 

    public Engine(IGame game, IGraphicsBackendFactory graphicsFactory)
    {
        _game = game;
        _graphicsFactory = graphicsFactory;

        _window = new Window();
        _systems = SystemDiscovery.Discover();

        var time = new Time();
        _loop = new GameLoop(time);

        _window.Load += () =>
        {
            var glContext = _window.NativeWindow.GLContext 
                ?? throw new InvalidOperationException("GLContext is null."); 

            var context = new GraphicsContext(name => glContext.GetProcAddress(name));

            _graphics = _graphicsFactory.Create(context);
            _textureManager = new TextureManager(_graphics);
            _game.Initialize(_graphics, _textureManager);
        };

        _window.Update += _ =>
        {
            _loop.Tick();

            var x = new SystemContext(time.DeltaTime);

            foreach (var systemInfo in _systems)
            {
                if (systemInfo.System is IUpdateSystem system)
                    system.Update(x);
            }

            _game.Update(time.DeltaTime);
        };

        _window.Render += _ =>
        {
            _graphics.Clear();

            // if (_renderer is not null)
            // {
            //     foreach (var tile in _loop.TilesToDraw)
            //     {
            //         _renderer.DrawTexture(tile.X, tile.Y, tile.Width, tile.Height);
            //     }
            // }

            _game.Render();
        };
    }

    public void Run()
        => _window.Run();

}