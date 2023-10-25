namespace Bell.Utils;

public class Logger
{
    public enum Level
    {
        Info,
        Warning,
        Error,
    }

    private List<ValueTuple<Level, string>> _logs = new();

    public void Info(string message)
    {
        _logs.Add(new(Level.Info, message));
    }
    
    public void Warning(string message)
    {
        _logs.Add(new(Level.Warning, message));
    }
    
    public void Error(string message)
    {
        _logs.Add(new(Level.Error, message));
    }
    
    public List<ValueTuple<Level, string>> GetLogs()
    {
        return _logs;
    }

    public void Clear()
    {
        _logs.Clear();
    }
}