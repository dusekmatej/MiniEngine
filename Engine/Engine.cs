using MiniEngine.Core;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Systems.Core;

namespace MiniEngine;

public class Engine
{
    private readonly Window _window; // Don't mistake the IWindow with Window class
    private readonly IGame _game;
    private readonly IGraphicsBackendFactory _graphicsFactory;
    private readonly IAssetSource? _assetSource;
    private readonly List<SystemInfo> _systems;


    private IGraphicsBackend? _graphics;
    private TextureAssets? _textureAssets;
    private FontManager? _fontManager;
    private Graphics2D? _drawing;

    // TESTING FIELD FOR SYSTEMS
    public Engine(
        IGame game,
        IGraphicsBackendFactory graphicsFactory,
        IAssetSource? assetSource = null)
    {
        _game = game;
        _graphicsFactory = graphicsFactory;
        _assetSource = assetSource;

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

        var context = new GraphicsContext(
            name => glContext.GetProcAddress(name),
            _window.NativeWindow.Size.X,
            _window.NativeWindow.Size.Y
        );

        _graphics = _graphicsFactory.Create(context);
        _textureAssets = new TextureAssets(_graphics, _assetSource);
        _fontManager = new FontManager();
        _drawing = new Graphics2D(_graphics, _textureAssets, _fontManager);

        _game.Initialize(_drawing, _textureAssets, _fontManager);
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

        _graphics.BeginFrame();
        _graphics.Clear();
        _game.Render();
        _graphics.EndFrame();
    }

}