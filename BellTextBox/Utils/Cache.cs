namespace Bell.Utils;

public class Cache<T>
{
    private T _value;
    private Func<T, T> _updateFunc;
    private bool _isDirty;

    public Cache(T initValue, Func<T, T> updateFunc)
    {
        _value = initValue;
        _updateFunc = updateFunc;
        _isDirty = true;
    }

    public T Get()
    {
        if (_isDirty)
            Update();
        
        return _value;
    }

    public void SetDirty() // TODO add time threshold
    {
        _isDirty = true;
    }

    private void Update()
    {
        _value = _updateFunc(_value);
        _isDirty = false;
    }
}