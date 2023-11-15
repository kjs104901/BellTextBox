using System.Text;

namespace Bell.Utils;

internal class CacheCounter
{
    private class Status
    {
        internal long GetCount;
        internal long SetDirtyCount;
        internal long UpdateCount;
    }
    
    private Dictionary<string, Status> _counter = new();
    
    internal void CountGet(string name)
    {
        if (false == _counter.ContainsKey(name))
            _counter.TryAdd(name, new Status());
        _counter[name].GetCount ++;
    }
    
    internal void CountSetDirty(string name)
    {
        if (false == _counter.ContainsKey(name))
            _counter.TryAdd(name, new Status());
        _counter[name].SetDirtyCount ++;
    }
    
    internal void CountUpdate(string name)
    {
        if (false == _counter.ContainsKey(name))
            _counter.TryAdd(name, new Status());
        _counter[name].UpdateCount ++;
    }
    
    internal string GetDebugString()
    {
        var sb = new StringBuilder();
        foreach (var (name, status) in _counter)
        {
            sb.AppendLine($"{name}: Get {status.GetCount}, SetDirty {status.SetDirtyCount}, Update {status.UpdateCount}");
        }
        return sb.ToString();
    }
}