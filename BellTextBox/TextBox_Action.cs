using Bell.Data;
using Action = Bell.Data.Action;

namespace Bell;

public partial class TextBox
{
    private const int HistoryCapacity = 1000;
    private readonly LinkedList<Action> _history = new();
    private readonly LinkedList<Action> _redoHistory = new();
    
    internal void DoAction(Action action)
    {
        action.DoCommands();
        
        _history.AddLast(action);
        if (_history.Count > HistoryCapacity)
        {
            _history.RemoveFirst();
        }
        _redoHistory.Clear();
    }
    
    internal void UndoAction()
    {
        if (_history.Last == null)
            return;

        var action = _history.Last.Value;
        action.UndoCommands();
        
        // TODO sequence undo
        
        _history.RemoveLast();
        _redoHistory.AddFirst(action);
    }

    internal void RedoAction()
    {
        if (_redoHistory.First == null)
            return;

        var action = _redoHistory.First.Value;
        action.DoCommands();

        // TODO sequence redo
        
        _redoHistory.RemoveLast();
        _history.AddLast(action);
    }

    internal string GetActionDebugString()
    {
        // TODO debug string
        throw new System.NotImplementedException();
    }
}