namespace Bell.Utils;

internal class Cache<T>
{
    private readonly string _name;
    private T _value;
    
    private readonly Func<T, T> _updateFunc;
    private bool _isDirty;

    private readonly Func<T, T>? _slowUpdateFunc;
    private bool _isSlowDirty;
    private DateTime _slowUpdateTime;
    private const int SlowUpdateInterval = 300;

    internal Cache(string name, T initValue, Func<T, T> updateFunc, Func<T, T>? slowUpdateFunc = null)
    {
        _name = name;
        _value = initValue;
        
        _updateFunc = updateFunc;
        _isDirty = true;
        
        _slowUpdateFunc = slowUpdateFunc;
        _isSlowDirty = true;
        _slowUpdateTime = DateTime.Now;
    }

    internal T Get()
    {
        if (_isDirty)
            Update();
        
        if (_isSlowDirty && DateTime.Now > _slowUpdateTime)
            SlowUpdate();

        if (DevHelper.IsDebugMode)
            Singleton.TextBox.CacheCounter.CountGet(_name);
        
        return _value;
    }

    internal void SetDirty()
    {
        if (DevHelper.IsDebugMode)
            Singleton.TextBox.CacheCounter.CountSetDirty(_name);
        
        _isDirty = true;

        _isSlowDirty = true;
        _slowUpdateTime = DateTime.Now.AddMilliseconds(SlowUpdateInterval);
    }

    private void Update()
    {
        if (DevHelper.IsDebugMode)
            Singleton.TextBox.CacheCounter.CountUpdate(_name);
        
        _value = _updateFunc(_value);
        _isDirty = false;
    }

    private void SlowUpdate()
    {
        if (null == _slowUpdateFunc)
            return;
        
        _value = _slowUpdateFunc(_value);
        _isSlowDirty = false;
    }
}