namespace MiniEngine.Database;

public class DatabaseStorage<T>
{
    private readonly Dictionary<string, T> _storage = new();
    public string DatabaseName { get; private set; } = typeof(T).Name;

    public void Import(string key, T value)
    {
        if (_storage.ContainsKey(key))
            throw new InvalidOperationException("Database error: Key already exists in the database storage.");

        if (value == null)
            throw new ArgumentNullException(nameof(value), "Database error: Value cannot be null.");
        Console.WriteLine($"Database: Imported {typeof(T).Name} with key {key} into {DatabaseName}");
        _storage.Add(key, value);
    }

    public T Get(string key)
    {
        if (!_storage.ContainsKey(key))
            throw new InvalidOperationException("Database error: Key not found in the database storage.");

        return _storage[key];
    }

    public bool TryGet(string key, out T? value) 
        => _storage.TryGetValue(key, out value);
}