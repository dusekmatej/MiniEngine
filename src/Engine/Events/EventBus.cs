namespace MiniEngine.Events;

public class EventBus
{
    private readonly Dictionary<Type, object> _listeners = new();

    public void Subscribe<T>(Action<T> listener)
    {
        ArgumentNullException.ThrowIfNull(listener);

        GetOrCreate<T>().Add(listener);
    }

    public bool Unsubscribe<T>(Action<T> listener)
    {
        ArgumentNullException.ThrowIfNull(listener);

        return Get<T>()?.Remove(listener) ?? false;
    }

    public void Publish<T>(T eventData)
        => Get<T>()?.Invoke(eventData);

    private EventListeners<T> GetOrCreate<T>()
    {
        var eventType = typeof(T);

        if (_listeners.TryGetValue(eventType, out var listeners))
            return (EventListeners<T>)listeners;

        var newListeners = new EventListeners<T>();
        _listeners.Add(eventType, newListeners);

        return newListeners;
    }

    private EventListeners<T>? Get<T>()
    {
        if (_listeners.TryGetValue(typeof(T), out var listeners))
            return (EventListeners<T>)listeners;

        return null;
    }
}
