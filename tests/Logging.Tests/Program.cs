using MiniEngine;
using MiniEngine.Logging;

{
    var log = new Logger("Test");
    var sink = new RecordingSink();
    log.AddSink(sink);

    var before = DateTimeOffset.Now;
    log.Debug("debug");
    log.Info("info");
    log.Warning("warning");
    log.Error("error");
    var after = DateTimeOffset.Now;

    Check(sink.Messages.Select(m => m.Level).SequenceEqual([LogLevel.Debug, LogLevel.Info, LogLevel.Warning, LogLevel.Error]),
        "Each method should write its own level.");
    Check(sink.Messages.Select(m => m.Message).SequenceEqual(["debug", "info", "warning", "error"]),
        "Messages should be passed unchanged.");
    Check(sink.Messages.All(m => m.Category == "Test"), "Messages should carry the logger category.");
    Check(sink.Messages.All(m => m.Timestamp >= before && m.Timestamp <= after), "Messages should carry the time they were created.");
}

{
    var log = new Logger("Test") { MinimumLevel = LogLevel.Warning };
    var sink = new RecordingSink();
    log.AddSink(sink);

    log.Debug("debug");
    log.Info("info");
    log.Warning("warning");
    log.Error("error");

    Check(sink.Messages.Select(m => m.Level).SequenceEqual([LogLevel.Warning, LogLevel.Error]),
        "Messages below the minimum level should be filtered out.");
    Check(!log.IsEnabled(LogLevel.Info) && log.IsEnabled(LogLevel.Error), "IsEnabled should follow the minimum level.");
}

{
    var log = new Logger("Engine");
    var sink = new RecordingSink();
    log.AddSink(sink);

    var assets = log.ForCategory("AssetPipeline");
    log.MinimumLevel = LogLevel.Info;

    assets.Debug("hidden");
    assets.Info("loaded");

    Check(sink.Messages.Count == 1 && sink.Messages[0].Category == "AssetPipeline",
        "Category loggers should share sinks and minimum level with their parent.");
    Check(log.Category == "Engine", "Creating a category logger should not change the parent.");
}

{
    var log = new Logger("Test");
    var first = new RecordingSink();
    var second = new RecordingSink();
    log.AddSink(first);
    log.AddSink(second);

    log.Info("both");
    Check(log.RemoveSink(first), "RemoveSink should report a removed sink.");
    log.Info("second only");

    Check(first.Messages.Count == 1 && second.Messages.Count == 2, "Every sink should receive messages until it is removed.");
    Check(!log.RemoveSink(first), "Removing an unknown sink should return false.");
}

{
    var exception = new InvalidOperationException("World file is corrupted");
    var message = new LogMessage(new DateTimeOffset(2026, 11, 11, 14, 5, 9, 42, TimeSpan.Zero),
        LogLevel.Error, "WorldGeneration", "Failed to load world", exception);

    string text = message.ToString();

    Check(text.StartsWith("[2026-11-11 14:05:09.042] [ERROR] [WorldGeneration] Failed to load world"),
        "Messages should use the shared format.");
    Check(text.Contains("InvalidOperationException") && text.Contains("World file is corrupted"),
        "The formatted message should include the exception.");

    var log = new Logger("Test");
    var sink = new RecordingSink();
    log.AddSink(sink);
    log.Error("Failed", exception);

    Check(sink.Messages[0].Exception == exception, "The exception should be passed to sinks.");
}

{
    var log = new Logger("Test");
    var sink = new RecordingSink();
    log.AddSink(new FailingSink());
    log.AddSink(sink);

    var error = new StringWriter();
    var originalError = Console.Error;
    Console.SetError(error);

    try
    {
        log.Info("still logged");
    }
    finally
    {
        Console.SetError(originalError);
    }

    Check(sink.Messages.Count == 1, "A failing sink should not stop the other sinks.");
    Check(error.ToString().Contains("FailingSink"), "A failing sink should be reported.");
}

{
    var log = new Logger("Console");
    log.AddSink(new ConsoleLogSink());

    var output = new StringWriter();
    var error = new StringWriter();
    var originalOutput = Console.Out;
    var originalError = Console.Error;
    Console.SetOut(output);
    Console.SetError(error);

    try
    {
        log.Info("World generated");
        log.Error("Failed to load world");
    }
    finally
    {
        Console.SetOut(originalOutput);
        Console.SetError(originalError);
    }

    Check(output.ToString().Contains("[INFO] [Console] World generated"), "Info should be written to standard output.");
    Check(error.ToString().Contains("[ERROR] [Console] Failed to load world"), "Errors should be written to standard error.");
    Check(!output.ToString().Contains("Failed to load world"), "Errors should not be written to standard output.");
}

{
    var log = new Logger("Threads");
    var sink = new RecordingSink();
    log.AddSink(sink);

    Parallel.For(0, 8, thread =>
    {
        var category = log.ForCategory($"Thread{thread}");

        for (int i = 0; i < 1000; i++)
            category.Info(i.ToString());
    });

    Check(sink.Messages.Count == 8000, "Concurrent writes should all reach the sink.");
}

{
    Check(Engine.Log.Category == "Engine", "Engine.Log should be available to games.");

    ExpectException<ArgumentException>(() => new Logger(" "));
    ExpectException<ArgumentException>(() => Engine.Log.ForCategory(""));
    ExpectException<ArgumentNullException>(() => new Logger("Test").AddSink(null!));
}

Console.WriteLine("Logging checks passed.");

static void Check(bool condition, string message)
{
    if (!condition)
        throw new Exception(message);
}

static void ExpectException<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new Exception($"Expected {typeof(TException).Name}.");
}

sealed class RecordingSink : ILogSink
{
    public List<LogMessage> Messages { get; } = new();

    public void Write(LogMessage message)
        => Messages.Add(message);
}

sealed class FailingSink : ILogSink
{
    public void Write(LogMessage message)
        => throw new IOException("Disk is full");
}
