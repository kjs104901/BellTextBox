using System.Text;
using Bell.Actions;
using Bell.Data;
using Action = Bell.Actions.Action;

namespace Bell;

public partial class TextBox
{
    private const int HistoryCapacity = 1000;
    private readonly LinkedList<Action> _actionHistory = new();
    private readonly LinkedList<Action> _actionRedoHistory = new();

    private void DoAction(Action action)
    {
        action.DoCommands();
        
        _actionHistory.AddLast(action);
        if (_actionHistory.Count > HistoryCapacity)
        {
            _actionHistory.RemoveFirst();
        }
        _actionRedoHistory.Clear();
    }

    private void UndoAction()
    {
        if (_actionHistory.Last == null)
            return;

        var lastAction = _actionHistory.Last.Value;
        lastAction.UndoCommands();
        _actionHistory.RemoveLast();
        _actionRedoHistory.AddFirst(lastAction);

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
        while (_actionHistory.Last != null &&
               _actionHistory.Last.Value.IsAllSame<T>())
        {
            var lastAction = _actionHistory.Last.Value;
            lastAction.UndoCommands();
            _actionHistory.RemoveLast();
            _actionRedoHistory.AddFirst(lastAction);
        }
    }

    private void RedoAction()
    {
        if (_actionRedoHistory.First == null)
            return;

        var firstAction = _actionRedoHistory.First.Value;
        firstAction.RedoCommands();
        _actionRedoHistory.RemoveFirst();
        _actionHistory.AddLast(firstAction);

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
        while (_actionRedoHistory.First != null &&
               _actionRedoHistory.First.Value.IsAllSame<T>())
        {
            var firstAction = _actionRedoHistory.First.Value;
            firstAction.RedoCommands();
            _actionRedoHistory.RemoveFirst();
            _actionHistory.AddLast(firstAction);
        }
    }

    internal string GetActionDebugString()
    {
        StringBuilder sb = new ();
        sb.AppendLine("History:");
        foreach (var action in _actionHistory)
        {
            sb.AppendLine(action.GetDebugString());
        }

        sb.AppendLine("Redo History:");
        foreach (var action in _actionRedoHistory)
        {
            sb.AppendLine(action.GetDebugString());
        }
        return sb.ToString();
    }
}