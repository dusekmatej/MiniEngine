namespace Database;

public static class Database
{
    private static readonly Dictionary<Type, object> _storages = new();


    // Get existing storage or create a new one for the specified type
    private static DatabaseStorage<T> GetStorage<T>()
    {
        var type = typeof(T);

        if (!_storages.TryGetValue(type, out var storage))
        {
            storage = new DatabaseStorage<T>();
            _storages[type] = storage;
        }
        return (DatabaseStorage<T>)storage;
    }



    public static void Load<T>(string key, T value)
        => GetStorage<T>().Import(key, value);

    public static T Get<T>(string key)
        => GetStorage<T>().Get(key);
}