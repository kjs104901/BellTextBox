using System.Text;

namespace Bell.Actions;

internal class ActionManager
{
    private readonly TextBox _textBox;
    
    private const int HistoryCapacity = 1000;
    private readonly LinkedList<Action> _history = new();
    private readonly LinkedList<Action> _redoHistory = new();

    public ActionManager(TextBox textBox)
    {
        _textBox = textBox;
    }

    public void Do(Action action)
    {
        action.DoCommands();
        
        _history.AddLast(action);
        if (_history.Count > HistoryCapacity)
        {
            _history.RemoveFirst();
        }
        _redoHistory.Clear();
    }
    
    public void Undo()
    {
        if (_history.Last == null)
            return;

        var action = _history.Last.Value;
        action.UndoCommands();
        
        // TODO sequence undo
        
        _history.RemoveLast();
        _redoHistory.AddFirst(action);
    }

    public void Redo()
    {
        if (_redoHistory.First == null)
            return;

        var action = _redoHistory.First.Value;
        action.DoCommands();

        // TODO sequence redo
        
        _redoHistory.RemoveLast();
        _history.AddLast(action);
    }

    public string GetDebugString()
    {
        // TODO debug string
        throw new System.NotImplementedException();
    }
}