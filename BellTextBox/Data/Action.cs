using System.Diagnostics;
using Bell.Utils;

namespace Bell.Data;

internal abstract class Action
{
    private readonly List<List<Command>> _caretsCommands = new();

    private readonly List<Caret> _startCarets = new();
    private readonly List<Caret> _endCarets = new();

    protected abstract List<Command> CreateCommands(Caret caret);
    
    public void DoCommands()
    {
        _caretsCommands.Clear();
        
        _startCarets.Clear();
        _endCarets.Clear();
        
        foreach (Caret caret in ThreadLocal.TextBox.Carets)
        {
            _startCarets.Add(caret.Clone());

            var commands = CreateCommands(caret);
            _caretsCommands.Add(commands);

            foreach (Command command in commands)
            {
                command.Do();
            }
            
            _endCarets.Add(caret.Clone());
        }
    }

    public void UndoCommands()
    {
        Debug.Assert(_endCarets.Count == _caretsCommands.Count);
        
        ThreadLocal.TextBox.Carets.Clear();
        ThreadLocal.TextBox.Carets.AddRange(_endCarets);
        
        for (int i = _caretsCommands.Count - 1; i >= 0; i--)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = ThreadLocal.TextBox.Carets[i];
            
            for (int j = commands.Count - 1; j >= 0; j--)
            {
                Command command = commands[j];
                // command.UndoAction(caret);
            }
        }
        
        ThreadLocal.TextBox.Carets.Clear();
        ThreadLocal.TextBox.Carets.AddRange(_startCarets);
    }

    public bool IsAllSame<T>()
    {
        return _caretsCommands
            .SelectMany(commands => commands)
            .All(command => command is T);
    }
    
    public string GetDebugString()
    {
        return string.Join(",", _caretsCommands.Select(a => a.GetType().ToString()));
    }
}

internal class DeleteSelection : Action
{
    public DeleteSelection()
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class InputCharAction : Action
{
    public InputCharAction(EditDirection direction)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteCharAction : Action
{
    public DeleteCharAction(EditDirection direction)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}
internal class EnterAction : Action
{
    public EnterAction()
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class TabAction : Action
{
    public TabAction()
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class UnTabAction : Action
{
    public UnTabAction()
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}