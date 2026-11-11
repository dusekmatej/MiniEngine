namespace MiniEngine.Logging;

public sealed class ConsoleLogSink : ILogSink
{
    public void Write(LogMessage message)
    {
        var writer = message.Level >= LogLevel.Warning
            ? Console.Error
            : Console.Out;

        writer.WriteLine(message.ToString());
    }
}
