# Systems
- This was developed for easiest possible use

## Dev notes
- I've made a system aimed for easy use, but customizability, there is implemented attribute usage for it,
there are two options for using the attributes, 
    - First option there was created an Enum which should completely be enough for most usecase, you use it trough [Priority(Low)] - Don't forget using static MiniEngine.Systems.Core.PriorityLevel - examples of priorities  
    - Second option use [Priority(100)] - you can use specific value if currently given options are not enough for your usecase
