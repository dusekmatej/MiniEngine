using MiniEngine.Graphics.Presets;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Graphics;
using MiniEngine.Core;
using MiniEngine.Game.Rendering;
using MiniEngine.Game.Environment;
using System.Numerics;
using Silk.NET.Input;

namespace MiniEngine.Game;

public class Game : IGame
{
    private Graphics2D? _drawing;
    private FontAssetHandle _debugFont;
    private TextureAssets? _textureAssets;
    private readonly IsometricPreset _preset = IsometricPreset.Default;
    private readonly MainScene _mainScene = new();
    private readonly Camera _camera = new(new Vector3(0f, -1.125f, 0f));
    private IKeyboard? _keyboard;
    private const float CameraSpeed = 2f;

    public void Initialize(Graphics2D graphics, TextureAssets textureAssets,
        FontManager fontManager, IInputContext input)
    {
        _textureAssets = textureAssets;
        _drawing = graphics;
        if (input.Keyboards.Count > 0)
        {
            _keyboard = input.Keyboards[0];
        }
        if (input.Mice.Count > 0)
        {
            input.Mice[0].Scroll += OnMouseScroll;
        }

        string fontPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts", "JetBrainsMono-Bold.ttf");
        _debugFont = fontManager.AddFont("debug", fontPath);
        Console.WriteLine("Game: Terrain explorer initialized. WASD / arrows, Shift to move faster, wheel to zoom, Home to reset.");
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
    {
        _camera.Zoom = Math.Clamp(_camera.Zoom * MathF.Pow(1.1f, scroll.Y), 0.25f, 4f);
    }

    public void Update(float deltaTime)
    {
        if (_keyboard is null)
        {
            return;
        }

        Vector3 movement = Vector3.Zero;
        if (_keyboard.IsKeyPressed(Key.W) || _keyboard.IsKeyPressed(Key.Up))
            movement.Y += 1f;
        if (_keyboard.IsKeyPressed(Key.S) || _keyboard.IsKeyPressed(Key.Down))
            movement.Y -= 1f;
        if (_keyboard.IsKeyPressed(Key.A) || _keyboard.IsKeyPressed(Key.Left))
            movement.X -= 1f;
        if (_keyboard.IsKeyPressed(Key.D) || _keyboard.IsKeyPressed(Key.Right))
            movement.X += 1f;

        if (movement != Vector3.Zero)
        {
            float speed = CameraSpeed / _camera.Zoom;
            if (_keyboard.IsKeyPressed(Key.ShiftLeft) || _keyboard.IsKeyPressed(Key.ShiftRight))
                speed *= 3f;
            _camera.Position += Vector3.Normalize(movement) * speed * deltaTime;
        }
        if (_keyboard.IsKeyPressed(Key.Home))
        {
            _camera.Position = new Vector3(0f, -1.125f, 0f);
            _camera.Zoom = 1f;
        }
    }

    public void Render()
    {
        if (_drawing is null || _textureAssets is null)
        {
            throw new InvalidOperationException("Game: Graphics have not been initialized.");
        }

        MainWorld world = _mainScene.MainWorld;
        world.UpdateVisibleChunks(_camera, _preset, _drawing.AspectRatio);
        int drawnTiles = Drawing.DrawWorld(_drawing, _textureAssets, world, _preset, _camera);

        _drawing.DrawRectangle(-1f, 0.77f, 2f, 0.23f, EngineColor.FromNormalized(0.04f, 0.06f, 0.09f, 0.9f));
        DrawHud("WASD / arrows: pan   Shift: faster   Wheel: zoom   Home: reset", 0.93f);
        DrawHud($"Chunks: {world.LoadedChunkCount} loaded  {world.GeneratedChunkCount} generated  {world.UnloadedChunkCount} unloaded   Tiles: {drawnTiles}", 0.86f);
        DrawHud($"Camera: {_camera.Position.X:F1}, {_camera.Position.Y:F1}   Zoom: {_camera.Zoom:F2}   View: {_drawing.ViewportWidth}x{_drawing.ViewportHeight}", 0.79f);
    }

    private void DrawHud(string text, float y)
    {
        if (_drawing is null || text.Length == 0)
        {
            return;
        }

        // JetBrains Mono advances by 0.6 em. Fit each line even in a tall window.
        const float characterAdvance = 0.6f;
        float aspectRatio = _drawing.AspectRatio;
        float height = Math.Min(0.045f, 1.9f * aspectRatio / (text.Length * characterAdvance));
        float advance = height * characterAdvance / aspectRatio;
        for (int i = 0; i < text.Length; i++)
        {
            _drawing.DrawText(_debugFont, text[i].ToString(), -0.97f + i * advance, y,
                height, EngineColor.White, scale: new Vector2(1f / aspectRatio, 1f));
        }
    }
}
