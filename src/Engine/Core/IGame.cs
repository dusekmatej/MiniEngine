using MiniEngine.Graphics;
using MiniEngine.Graphics.Fonts;

namespace MiniEngine.Core;

public interface IGame
{
    public void Initialize(
        Graphics2D graphics,
        TextureAssets textureAssets,
        FontManager fontManager);
    public void Update(float deltaTime);
    public void Render();
}