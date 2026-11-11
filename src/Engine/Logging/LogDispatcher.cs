namespace MiniEngine.Logging;

internal sealed class LogDispatcher
{
    private readonly object _lock = new();
    private readonly List<ILogSink> _sinks = new();

    public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

    public bool IsEnabled(LogLevel level)
        => level >= MinimumLevel;

    public void AddSink(ILogSink sink)
    {
        lock (_lock)
            _sinks.Add(sink);
    }

    public bool RemoveSink(ILogSink sink)
    {
        lock (_lock)
            return _sinks.Remove(sink);
    }

    public void Write(LogMessage message)
    {
        if (!IsEnabled(message.Level))
            return;

        lock (_lock)
        {
            foreach (var sink in _sinks)
            {
                try
                {
                    sink.Write(message);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Log sink {sink.GetType().Name} failed: {e}");
                }
            }
        }
    }
}
