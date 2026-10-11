using System.Numerics;
using MiniEngine.Graphics.Presets;

namespace MiniEngine.Graphics;

public class IsometricProjection
{
    public IsometricPreset Preset { get; }

    public IsometricProjection(IsometricPreset preset)
    {
        Preset = preset;
    }

    public Vector2 WorldToScreen(Vector3 worldPosition, Camera camera, Vector2 screenOrigin)
    {
        Vector3 relativePosition = worldPosition - camera.Position;

        float screenX = (relativePosition.X - relativePosition.Y) * Preset.FootprintWidth / 2f;
        float screenY = -(relativePosition.X + relativePosition.Y) * Preset.FootprintHeight / 2f;

        screenY += relativePosition.Z * Preset.ElevationStep;

        return screenOrigin + new Vector2(screenX * camera.Zoom, screenY * camera.Zoom);
    }

    public Vector2 GetSpriteSize(Camera camera)
    {
        return new Vector2(Preset.SpriteWidth * camera.Zoom, Preset.SpriteHeight * camera.Zoom);
    }
}