namespace Bell.Utils;

internal class Cache<T>
{
    private T _value;
    private Func<T, T> _updateFunc;
    private bool _isDirty;

    internal Cache(T initValue, Func<T, T> updateFunc)
    {
        _value = initValue;
        _updateFunc = updateFunc;
        _isDirty = true;
    }

    internal T Get()
    {
        if (_isDirty)
            Update();
        
        return _value;
    }

    internal void SetDirty() // TODO add time threshold
    {
        _isDirty = true;
    }

    private void Update()
    {
        _value = _updateFunc(_value);
        _isDirty = false;
    }
}