using MiniEngine.Core;
using MiniEngine.Game;
using MiniEngine.OpenGL.Core;

var game = new Game();
var graphicsFactory = new BackendFactory();

var engine = new Engine(game, graphicsFactory);

engine.Run();