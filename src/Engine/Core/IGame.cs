using MiniEngine.Graphics;
using MiniEngine.Graphics.Fonts;
using MiniEngine.Input;

namespace MiniEngine.Core;

public interface IGame
{
    public void Initialize(
        Graphics2D graphics,
        TextureAssets textureAssets,
        FontManager fontManager,
        KeyboardInput keyboard,
        MouseInput mouse);
    public void Update(float deltaTime);
    public void Render();
}