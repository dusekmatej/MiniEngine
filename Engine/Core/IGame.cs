namespace Engine.Core;

public interface IGame
{
    public void Initialize();
    public void Update(float deltaTime);
    public void Render();
}