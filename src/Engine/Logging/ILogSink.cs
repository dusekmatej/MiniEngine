namespace MiniEngine.Logging;

public interface ILogSink
{
    void Write(LogMessage message);
}
