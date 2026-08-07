using MiniEngine.Engine.Graphics;

namespace MiniEngine.Engine.Core;

public interface IGame
{
    public void Initialize(Renderer renderer);
    public void Update(float deltaTime);
    public void Render();
}