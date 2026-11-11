namespace MiniEngine.Logging;

public sealed class Logger
{
    private readonly LogDispatcher _dispatcher;

    public string Category { get; }

    public LogLevel MinimumLevel
    {
        get => _dispatcher.MinimumLevel;
        set => _dispatcher.MinimumLevel = value;
    }

    public Logger(string category)
        : this(new LogDispatcher(), category)
    {
    }

    private Logger(LogDispatcher dispatcher, string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        _dispatcher = dispatcher;
        Category = category;
    }

    public Logger ForCategory(string category)
        => new(_dispatcher, category);

    public void AddSink(ILogSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        _dispatcher.AddSink(sink);
    }

    public bool RemoveSink(ILogSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        return _dispatcher.RemoveSink(sink);
    }

    public bool IsEnabled(LogLevel level)
        => _dispatcher.IsEnabled(level);

    public void Debug(string message, Exception? exception = null)
        => Write(LogLevel.Debug, message, exception);

    public void Info(string message, Exception? exception = null)
        => Write(LogLevel.Info, message, exception);

    public void Warning(string message, Exception? exception = null)
        => Write(LogLevel.Warning, message, exception);

    public void Error(string message, Exception? exception = null)
        => Write(LogLevel.Error, message, exception);

    public void Write(LogLevel level, string message, Exception? exception = null)
    {
        if (!_dispatcher.IsEnabled(level))
            return;

        var logMessage = new LogMessage(
            DateTimeOffset.Now,
            level,
            Category,
            message ?? string.Empty,
            exception
        );

        _dispatcher.Write(logMessage);
    }
}
