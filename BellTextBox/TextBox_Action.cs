using Bell.Data;
using Action = Bell.Data.Action;

namespace Bell;

public partial class TextBox
{
    private const int HistoryCapacity = 1000;
    private readonly LinkedList<Action> _history = new();
    private readonly LinkedList<Action> _redoHistory = new();

    private void DoAction(Action action)
    {
        action.DoCommands();
        
        _history.AddLast(action);
        if (_history.Count > HistoryCapacity)
        {
            _history.RemoveFirst();
        }
        _redoHistory.Clear();
    }

    private void UndoAction()
    {
        if (_history.Last == null)
            return;

        var lastAction = _history.Last.Value;
        lastAction.UndoCommands();
        _history.RemoveLast();
        _redoHistory.AddFirst(lastAction);

        if (lastAction.IsAllSame<InputCharCommand>())
        {
            UndoActionSequence<InputCharCommand>();
        }
        else if (lastAction.IsAllSame<DeleteCharCommand>())
        {
            UndoActionSequence<DeleteCharCommand>();
        }
    }
    
    private void UndoActionSequence<T>()
    {
        while (_history.Last != null &&
               _history.Last.Value.IsAllSame<T>())
        {
            var lastAction = _history.Last.Value;
            lastAction.UndoCommands();
            _history.RemoveLast();
            _redoHistory.AddFirst(lastAction);
        }
    }

    private void RedoAction()
    {
        if (_redoHistory.First == null)
            return;

        var firstAction = _redoHistory.First.Value;
        firstAction.DoCommands();
        _redoHistory.RemoveLast();
        _history.AddLast(firstAction);

        if (firstAction.IsAllSame<InputCharCommand>())
        {
            RedoActionSequence<InputCharCommand>();
        }
        else if (firstAction.IsAllSame<DeleteCharCommand>())
        {
            RedoActionSequence<DeleteCharCommand>();
        }
    }
    
    private void RedoActionSequence<T>()
    {
        while (_redoHistory.First != null &&
               _redoHistory.First.Value.IsAllSame<T>())
        {
            var firstAction = _redoHistory.First.Value;
            firstAction.DoCommands();
            _redoHistory.RemoveFirst();
            _history.AddLast(firstAction);
        }
    }

    internal string GetActionDebugString()
    {
        // TODO debug string
        throw new System.NotImplementedException();
    }
}