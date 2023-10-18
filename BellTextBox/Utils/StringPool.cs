namespace Bell.Utils;

public static class StringPool<T> where T : notnull
{
    private static readonly Dictionary<T, string> Pool = new();
    
    public static string Get(T key)
    {
        if (Pool.TryGetValue(key, out string? value))
            return value;
        
        value = key.ToString() ?? string.Empty;
        Pool.Add(key, value);
        return value;
    }
}