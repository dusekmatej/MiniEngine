using System.Numerics;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Fonts;

namespace MiniEngine.Game;

public static class Drawing
{
    private static bool _demoButtonWasPressed;

    public static void DrawPremadeItems(Graphics2D _drawing, FontAssetHandle? _debugFont, TextureAssetHandle? _tileTexture)
    {   
        if (_drawing is null || _debugFont is null)
            return;

        _drawing.DrawRectangle(
            -1f,
            -1f,
            2f,
            2f,
            EngineColor.FromNormalized(0.08f, 0.09f, 0.14f),
            layer: -10
        );

        _drawing.DrawText(
            _debugFont.Value,
            "DrawCommands demo",
            -0.88f,
            0.82f,
            0.10f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Triangle",
            -0.88f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawTriangle(
            -0.78f,
            0.25f,
            0.22f,
            0.22f,
            EngineColor.Green,
            rotation: 0.18f,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Circle",
            -0.52f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawCircle(
            -0.40f,
            0.35f,
            0.10f,
            EngineColor.Cyan,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Line",
            -0.15f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawLine(
            -0.15f,
            0.33f,
            0.10f,
            0.33f,
            0.025f,
            EngineColor.Red,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Rectangle",
            0.18f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawRectangle(
            0.28f,
            0.25f,
            0.25f,
            0.18f,
            EngineColor.Yellow,
            rotation: 0.12f,
            scale: new Vector2(1f, 0.85f),
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Outline",
            0.58f,
            0.52f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawRectangleOutline(
            new Rectangle(0.62f, 0.25f, 0.22f, 0.18f),
            EngineColor.Magenta,
            thickness: 0.02f,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Circle outline",
            -0.88f,
            -0.02f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        _drawing.DrawCircle(
            -0.70f,
            -0.18f,
            0.10f,
            EngineColor.Magenta,
            filled: false,
            layer: 2
        );

        _drawing.DrawText(
            _debugFont.Value,
            "Sprite",
            -0.35f,
            -0.02f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        if (_tileTexture is TextureAssetHandle texture)
        {
            _drawing.DrawSprite(
                texture,
                -0.28f,
                -0.20f,
                0.20f,
                0.20f,
                EngineColor.White,
                layer: 2
            );
        }

        _drawing.DrawText(
            _debugFont.Value,
            "Button",
            0.12f,
            -0.02f,
            0.065f,
            EngineColor.White,
            layer: 10
        );

        Rectangle buttonBounds = new Rectangle(0.10f, -0.25f, 0.48f, 0.20f);
        bool buttonPressed = _drawing.DrawButton(
            buttonBounds,
            "Click me",
            EngineColor.FromRgb(20, 90, 190),
            EngineColor.White,
            _debugFont.Value
        );

        if (buttonPressed && !_demoButtonWasPressed)
            Console.WriteLine("Hello");

        _demoButtonWasPressed = buttonPressed;
    }
}