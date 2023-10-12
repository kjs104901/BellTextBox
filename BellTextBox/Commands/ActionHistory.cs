using System.Text;

namespace Bell.Commands;

internal class ActionHistory
{
    private const int Capacity = 1000;
    private readonly LinkedList<Action> _history = new();
    private readonly LinkedList<Action> _redoHistory = new();

    public void AddHistory(Action action)
    {
        if (action.HasEditAction)
        {
            _history.AddLast(action);
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
        foreach (Action actionSet in _history)
        {
            sb.Append("[");
            sb.Append(actionSet.GetDebugString());
            sb.Append("]");
            sb.AppendLine();
        }
        sb.AppendLine("Redo History");
        foreach (Action actionSet in _redoHistory)
        {
            sb.Append("[");
            sb.Append(actionSet.GetDebugString());
            sb.Append("]");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}