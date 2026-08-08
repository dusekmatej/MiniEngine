using MiniEngine.Graphics;

namespace MiniEngine.Core;

public interface IGame
{
    public void Initialize(IGraphicsBackend graphics);
    public void Update(float deltaTime);
    public void Render();
}