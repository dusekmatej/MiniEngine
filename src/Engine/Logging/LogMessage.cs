namespace MiniEngine.Logging;

public readonly record struct LogMessage(
    DateTimeOffset Timestamp,
    LogLevel Level,
    string Category,
    string Message,
    Exception? Exception = null)
{
    public override string ToString()
    {
        var level = Level.ToString().ToUpperInvariant();
        var line = $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{Category}] {Message}";

        if (Exception is null)
            return line;

        return line + System.Environment.NewLine + Exception;
    }
}
