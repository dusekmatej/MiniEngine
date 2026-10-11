# MiniEngine

**MiniEngine** is an open-source 2D game engine focused on building **lightweight, fast and approachable isometric and top-down games**.

The project is written in **C#** and is currently under active development.

MiniEngine is not trying to become another huge general-purpose engine. The idea is to build something smaller and more focused — an engine where you can understand what is happening, keep control over your game, and avoid pulling in an enormous runtime for projects that simply do not need one.

If your attention span is smaller than this document, then jump to the Open-Source section if you would want to collaborate

> **MiniEngine is still early in development.**  
> "APIs", architecture and systems are actively evolving.

---

## Why MiniEngine?

Modern engines such as Unity are incredibly capable, but that flexibility comes with a lot of systems, abstractions and overhead that many smaller 2D games simply do not need.

MiniEngine is exploring a different direction.

The goal is an engine designed specifically around games such as:

- isometric games
- top-down games
- strategy games
- simulation games
- management games
- tile-based worlds
- smaller 2D projects where simplicity and performance matter

Instead of supporting every possible type of game, MiniEngine can concentrate on doing a smaller set of things well.

### The goals

MiniEngine aims to provide:

- **Fast startup and loading times**
- **Low runtime overhead**
- **A lightweight engine architecture**
- **A clean C# API**
- **A data-oriented ECS**
- **Simple scene and world management**
- **Efficient 2D rendering**
- **Straightforward asset loading**
- **Strong support for tile-based, top-down and isometric worlds**
- **An engine architecture that developers can actually understand**

A major goal is keeping the engine considerably smaller and more focused than large general-purpose engines such as Unity.

You should not need an enormous engine just to make a small 2D game.

---

## Current Development

MiniEngine is still being built and many systems are evolving quickly.

Some of the areas currently being developed include:

- Entity Component System
- automatic system discovery
- scenes and worlds
- 2D rendering
- texture and image management
- asset loading
- terrain and world generation
- procedural terrain generation
- game-side engine configuration

A lot of work is also going into making sure the engine itself and the actual **Game** project remain properly separated.

The intention is that engine users should be able to create their game without constantly fighting or modifying engine internals.

---

## Philosophy

There are a few ideas behind MiniEngine that influence most architectural decisions.

### Keep the engine understandable

Abstraction is useful until it hides everything.

MiniEngine should have good abstractions, but developers interested in the engine should still be able to follow the path from their game code all the way down to the systems doing the actual work.

### Build for a specific type of game

MiniEngine does not need to render AAA first-person shooters.

By focusing primarily on **2D top-down and isometric games**, the engine can make stronger assumptions and build better tools for those games.

### Keep the game project clean

Game-specific code and engine code should remain separate.

Ideally, creating a game with MiniEngine should feel like using a library rather than modifying an engine.

### Avoid unnecessary systems

Features should exist because games need them, not simply because other engines have them.

---

## Open Source

MiniEngine is an **open-source project**, and I would love for it to become more than a one-person experiment.

If game development, engine architecture, rendering, ECS design, procedural generation, tooling or simply experimenting with game engine ideas sounds interesting to you, you are very welcome here.

You do **not** need to be an experienced engine developer.

I'm especially interested in meeting people who enjoy learning how engines work and want to build something together.

There are plenty of areas that could use different perspectives:

- Engine architecture
- ECS design
- Rendering
- Asset pipelines
- Procedural generation
- Terrain systems
- Performance
- Developer experience
- Documentation
- Testing
- Example games
- Tools

Even reviewing architecture, discussing an idea or pointing out something that could be designed better is valuable.

---

## Contributing

MiniEngine is currently evolving quickly, so before starting a large implementation it is a good idea to talk about the idea first.

You can:

1. **Open a GitHub Issue** for bugs, feature ideas or architectural suggestions.
2. **Start a GitHub Discussion** if Discussions are enabled and you want to talk about a broader idea.
3. Fork the repository and experiment with something.
4. Open a Pull Request with an improvement.
5. Reach out directly if you are interested in working on the engine more regularly.

If you see something badly designed, unnecessarily complicated, slow, confusing or simply annoying to use, please say so.

That kind of feedback is extremely useful while the engine is still young.

---

## About Me

Hi! I'm **Matej Dusek** a.k.a. matesoidicek, a student building MiniEngine in my free time.

I started the project because I enjoy understanding how systems work underneath the abstractions we normally use when making games.

A big part of the fun for me is designing things like engine architecture, ECS systems, asset management, databases, procedural systems and the connections between all of them.

MiniEngine is both a project I genuinely want to use for games and a way for me to learn more about **game development, game engines, graphics and software architecture**.

I'm looking for people who have a similar interest in these areas and would enjoy building, discussing and experimenting with an engine together.

You do not need to treat this like a job.

It's a passion project.

---

## Want to Work on MiniEngine?

If the project sounds interesting, **please reach out**.

The easiest option is usually opening an Issue or Discussion directly on GitHub so other contributors can participate in the conversation as well.

For private conversations, collaboration ideas or anything that does not fit into an Issue:

**Email:** [mat.dusek@gmail.com](mailto:mat.dusek@gmail.com)

If the project grows, communication will probably move toward GitHub Discussions or another community space, but for now keeping everything close to the repository makes the most sense.

---

## Project Status

🚧 **Early Development**

MiniEngine is not currently intended for production games.

Expect:

- unfinished systems
- API changes
- architectural changes
- missing documentation
- experiments that may later be replaced entirely

This is actually one of the best times to contribute because the important architectural decisions are **still being made**.

---

## Long-Term Vision

The long-term goal is for MiniEngine to become a small but capable engine where creating a game looks something like this:

**Create a project → define your worlds and scenes → add your assets → build your game.**

Without enormous loading times.

Without hundreds of systems running that your game never uses.

Without needing to understand an editor before you can understand your own project.

Just a focused engine for making **2D worlds**.

---

## License

The project is open source.

See the repository's `LICENSE` file for details.

---

### Interested?

⭐ Star the repository if you want to follow the project.

🐛 Open an Issue if you find something wrong.

💡 Share an idea if you think something could be better.

🔧 Open a Pull Request if you want to build something.

And if you are another student, game developer, engine developer, or simply someone who likes figuring out how these systems work:

**come build MiniEngine with me.**


NOTE: this was written with AI because I didn't have time for writing it myself, ill fix it soon