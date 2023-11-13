﻿namespace Bell.Utils;

internal class Cache<T>
{
    private readonly string _name;
    private T _value;
    
    private readonly Func<T, T> _updateFunc;
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

        if (DevHelper.IsDebugMode)
            Singleton.TextBox.CacheCounter.CountGet(_name);
        
        return _value;
    }

    internal void SetDirty()
    {
        if (DevHelper.IsDebugMode)
            Singleton.TextBox.CacheCounter.CountSetDirty(_name);
        
        _isDirty = true;
    }

    private void Update()
    {
        if (DevHelper.IsDebugMode)
            Singleton.TextBox.CacheCounter.CountUpdate(_name);
        
        _value = _updateFunc(_value);
        _isDirty = false;
    }
}