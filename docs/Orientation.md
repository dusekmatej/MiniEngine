# Orientation
- Theese notes will not be used as the final docs, this is just the orientation notes for everyone that is on-boarded, trough-out the school year.

# Current Engine usage

### Game.cs
It is the main entry-point, where the user will start configuring aything. This part won't be changed it will always stay as the main entry point.

### Scene & World
First I will start with the Engine Side, **Scene** and **IScene**, you can imagine theese as the guidelines of how should every **Scene** created inside of Game Side look. Every time inside of the Game you will be inheriting from **Scene** not IScene. When you create a scene inside of Game project be sure that you don't name it "Scene" name it something like **"MainScene"**, so there is no confusion. Now I will move to the World, but before that I need to mention that every world has to be Owned by a Scene, so we currently have a relationship between **MainScene <--- MainWorld**. 

### Rendering
We have multiple possible options to call for rendering. Here is the list: DrawWorld, DrawChunk, DrawPlatform, DrawPremadeItems Importantly this is not the real backend drawing calls theese are just the Drawing calls inside of Game project. Then we have the backend calls which are: DrawRectangle, DrawText, DrawTriangle, DrawCircle, DrawRectangleOutline, DrawSprite, DrawButton. For now if you will want to use one of theese you will need to inspect them more in depth to figure out what does what. On the end of the year we will hopefully have real documentation. If anyone wants to create just documentation instead of writing code then you are free to do that. 

### Input
We currently handle input very poorly, that will be hopefuly changed so its not worth to mention, however just to be sure it is inside of the Game.cs. 

## Entity Component System (ECS)
The engine is built for using ECS, which is practically 