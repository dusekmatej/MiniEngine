using MiniEngine.Graphics;
using MiniEngine.Systems.Core;

namespace MiniEngine.Core;

public class Engine
{
    private readonly Window _window; // Don't mistake the IWindow with Window class
    private readonly IGame _game;
    private readonly IGraphicsBackendFactory _graphicsFactory;
    private readonly List<SystemInfo> _systems;


    private IGraphicsBackend? _graphics;
    private TextureManager? _textureManager;

    // TESTING FIELD FOR SYSTEMS
    public Engine(IGame game, IGraphicsBackendFactory graphicsFactory)
    {
        _game = game;
        _graphicsFactory = graphicsFactory;

        _window = new Window();
        _systems = SystemDiscovery.Discover();

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
    }

    public void Run()
        => _window.Run();

    private void OnLoad()
    {
        var glContext = _window.NativeWindow.GLContext 
                ?? throw new InvalidOperationException("GLContext is null."); 

        var context = new GraphicsContext(name => glContext.GetProcAddress(name));

        _graphics = _graphicsFactory.Create(context);
        _textureManager = new TextureManager(_graphics);

        _game.Initialize(_graphics, _textureManager);
    }

    private void OnUpdate(double deltaTime)
    {
        var x = new SystemContext((float)deltaTime);

        foreach (var systemInfo in _systems)
        {
            if (systemInfo.System is IUpdateSystem system)
                system.Update(x);
        }

        _game.Update((float)deltaTime);
    }

    private void OnRender(double _)
    {
        if (_graphics is null)
            return;

        _graphics.Clear();
        _game.Render();
    }

}