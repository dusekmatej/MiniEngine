using static MiniEngine.Systems.Core.PriorityLevel;
using MiniEngine.Systems.Core;

namespace MiniEngine.Systems;

[Priority(Highest)]
public class HelloWorldSystem : IUpdateSystem
{
    public void Update(SystemContext x)
    {
    }
}