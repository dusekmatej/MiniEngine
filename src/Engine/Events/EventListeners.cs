namespace MiniEngine.Events;

internal sealed class EventListeners<T>
{
    private Action<T>[] _listeners = [];

    public void Add(Action<T> listener)
        => _listeners = [.. _listeners, listener];

    public bool Remove(Action<T> listener)
    {
        int index = Array.LastIndexOf(_listeners, listener);

        if (index < 0)
            return false;

        var updated = new Action<T>[_listeners.Length - 1];

        Array.Copy(_listeners, 0, updated, 0, index);
        Array.Copy(_listeners, index + 1, updated, index, updated.Length - index);

        _listeners = updated;
        return true;
    }

    public void Invoke(T eventData)
    {
        var listeners = _listeners;
        List<Exception>? exceptions = null;

        foreach (var listener in listeners)
        {
            try
            {
                listener(eventData);
            }
            catch (Exception e)
            {
                exceptions ??= new List<Exception>();
                exceptions.Add(e);
            }
        }

        if (exceptions is not null)
            throw new AggregateException(
                $"{exceptions.Count} listener(s) of {typeof(T).Name} threw an exception.", exceptions);
    }
}
