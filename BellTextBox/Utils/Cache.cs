namespace Bell.Utils;

internal class Cache<T>
{
    private string _name;
    private T _value;
    private Func<T, T> _updateFunc;
    private bool _isDirty;

    internal Cache(string name, T initValue, Func<T, T> updateFunc)
    {
        _name = name;
        _value = initValue;
        _updateFunc = updateFunc;
        _isDirty = true;
    }

    internal T Get()
    {
        if (_isDirty)
            Update();
        
        Singleton.TextBox.CacheCounter.CountGet(_name);
        return _value;
    }

    internal void SetDirty() // TODO add time threshold
    {
        Singleton.TextBox.CacheCounter.CountSetDirty(_name);
        _isDirty = true;
    }

    private void Update()
    {
        Singleton.TextBox.CacheCounter.CountUpdate(_name);
        _value = _updateFunc(_value);
        _isDirty = false;
    }
}