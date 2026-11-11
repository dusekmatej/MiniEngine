# Logging

Use `Engine.Log` instead of `Console.WriteLine`. It is available from the engine
and from the game:

```csharp
using MiniEngine;

Engine.Log.Info("World generated");
Engine.Log.Warning("Texture not found");
Engine.Log.Error("Failed to load world", exception);
Engine.Log.Debug("Chunk generated");
```

`Engine.Log` writes with the `Engine` category. For other categories create a
category logger once and keep it:

```csharp
private static readonly Logger Log = Engine.Log.ForCategory("WorldGeneration");

Log.Info("Chunk 0,0 generated");
```

## Levels

| Level | Use for |
| --- | --- |
| `Debug` | detailed information for development |
| `Info` | normal information about what the engine does |
| `Warning` | unexpected situations that do not stop the game |
| `Error` | failures that prevented an operation from finishing |

Set the minimum level to hide less important messages. With `Warning` only
warnings and errors are written:

```csharp
Engine.Log.MinimumLevel = LogLevel.Warning;
```

The default is `Debug`. Category loggers share the minimum level and sinks with
the logger they were created from.

## Format

Every message has a timestamp, level, category, text and an optional exception:

```
[2026-11-11 14:05:09.042] [ERROR] [WorldGeneration] Failed to load world
System.InvalidOperationException: World file is corrupted
   at ...
```

`LogMessage.ToString()` produces this format, so every sink can use the same one.

## Sinks

A sink is where messages are written. `Engine.Log` starts with
`ConsoleLogSink`, which writes `Debug` and `Info` to standard output and
`Warning` and `Error` to standard error.

Implement `ILogSink` to add another output, for example a file or an editor
panel:

```csharp
public sealed class FileLogSink(string path) : ILogSink
{
    public void Write(LogMessage message)
        => File.AppendAllText(path, message + Environment.NewLine);
}

Engine.Log.AddSink(new FileLogSink("game.log"));
```

Sinks are called one message at a time under a lock, so a sink does not have to
be thread-safe and lines from different threads never mix. If a sink throws,
the error is reported on standard error and the other sinks still receive the
message. Logging itself never throws because of a sink.

## Validation

Run the dependency-free console checks with:

```sh
dotnet run --project tests/Logging.Tests/Logging.Tests.csproj
```
