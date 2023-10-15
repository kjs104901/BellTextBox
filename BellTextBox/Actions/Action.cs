using System.Diagnostics;
using Bell.Carets;

namespace Bell.Actions;

internal abstract class Action
{
    private readonly TextBox _textBox;
    
    private readonly List<List<Command>> _caretsCommands = new();

    private readonly List<Caret> _startCarets = new();
    private readonly List<Caret> _endCarets = new();

    protected Action(TextBox textBox)
    {
        _textBox = textBox;
    }

    protected abstract List<Command> CreateCommands(Caret caret);
    
    public void DoCommands()
    {
        _caretsCommands.Clear();
        
        _startCarets.Clear();
        _endCarets.Clear();
        
        foreach (Caret caret in _textBox.CaretManager.Carets)
        {
            _startCarets.Add(caret.Clone());

            var commands = CreateCommands(caret);
            _caretsCommands.Add(commands);

            foreach (Command command in commands)
            {
                // command.Do(caret);
            }
            
            _endCarets.Add(caret.Clone());
        }
    }

    public void UndoCommands()
    {
        Debug.Assert(_endCarets.Count == _caretsCommands.Count);
        
        _textBox.CaretManager.Carets.Clear();
        _textBox.CaretManager.Carets.AddRange(_endCarets);
        
        for (int i = _caretsCommands.Count - 1; i >= 0; i--)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = _textBox.CaretManager.Carets[i];
            
            for (int j = commands.Count - 1; j >= 0; j--)
            {
                Command command = commands[j];
                // command.Undo(caret);
            }
        }
        
        _textBox.CaretManager.Carets.Clear();
        _textBox.CaretManager.Carets.AddRange(_startCarets);
    }
    
    public string GetDebugString()
    {
        return string.Join(",", _caretsCommands.Select(a => a.GetType().ToString()));
    }
}

internal class DeleteSelection : Action
{
    public DeleteSelection(TextBox textBox) : base(textBox)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class InputCharAction : Action
{
    public InputCharAction(TextBox textBox, EditDirection direction) : base(textBox)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteCharAction : Action
{
    public DeleteCharAction(TextBox textBox, EditDirection direction) : base(textBox)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}
internal class EnterAction : Action
{
    public EnterAction(TextBox textBox) : base(textBox)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class TabAction : Action
{
    public TabAction(TextBox textBox) : base(textBox)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}

internal class UnTabAction : Action
{
    public UnTabAction(TextBox textBox) : base(textBox)
    {
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        throw new NotImplementedException();
    }
}