using MiniEngine.Graphics;

namespace MiniEngine.Core;

public interface IGame
{
    public void Initialize(Renderer renderer);
    public void Update(float deltaTime);
    public void Render();
}