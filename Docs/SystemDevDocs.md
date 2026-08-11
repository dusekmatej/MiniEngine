# Systems
- This was developed for easiest possible use

## Use notes
- I've made a system aimed for easy use, but customizability, there is implemented attribute usage for it,
there are two options for using the attributes, 
    - First option there was created an Enum which should completely be enough for most usecase, you use it trough [Priority(Low)] - Don't forget using static MiniEngine.Systems.Core.PriorityLevel - examples of priorities  
    - Second option use [Priority(100)] - you can use specific value if currently given options are not enough for your usecase

# Development
- ```ISystem``` - main interface for every other system, it is mainly used for identification of systems
- ```IUpdateSystem``` - This is a system template that inherits interface ```ISystem``` and contains Update method which is called every current deltaTime
- ```RenderContext``` - Context in which you can set what you will exchange between systems
- ```AssemblyDiscovery```, ```SystemDiscovery``` & ```SystemFactory``` - are used for creating and automatically finding systems based on the interfaces each inherits
- ```PriorityLevel``` & ```PriorityAttribute``` - are used for determining how important each system is by just writing [Priority(PriorityLevel.LEVEL)] or [Priority(100)]
