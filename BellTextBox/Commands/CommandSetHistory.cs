using System.Text;

namespace Bell.Commands;

internal class CommandSetHistory
{
    private const int Capacity = 1000;
    private readonly LinkedList<CommandSet> _history = new();
    private readonly LinkedList<CommandSet> _redoHistory = new();

    public void AddHistory(CommandSet commandSet)
    {
        if (commandSet.HasEditAction)
        {
            _history.AddLast(commandSet);
            if (_history.Count > Capacity)
            {
                _history.RemoveFirst();
            }
            _redoHistory.Clear();
        }
    }
    
    public void Undo()
    {
        if (_history.Last == null)
            return;

        var action = _history.Last.Value;

        _redoHistory.AddFirst(action);
    }

    public void Redo()
    {
        if (_redoHistory.First == null)
            return;

        var action = _redoHistory.First.Value;

        _history.AddLast(action);
    }

    public string GetDebugString()
    {
        StringBuilder sb = new();
        sb.AppendLine("History");
        foreach (CommandSet actionSet in _history)
        {
            sb.Append("[");
            sb.Append(actionSet.GetDebugString());
            sb.Append("]");
            sb.AppendLine();
        }
        sb.AppendLine("Redo History");
        foreach (CommandSet actionSet in _redoHistory)
        {
            sb.Append("[");
            sb.Append(actionSet.GetDebugString());
            sb.Append("]");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}