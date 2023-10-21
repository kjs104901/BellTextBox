using System.Diagnostics;
using System.Text;
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
                command.Do(caret);
            }

            _endCarets.Add(caret.Clone());
        }
    }

    public void UndoCommands()
    {
        ThreadLocal.TextBox.Carets.Clear();
        ThreadLocal.TextBox.Carets.AddRange(_endCarets);

        for (int i = _caretsCommands.Count - 1; i >= 0; i--)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = ThreadLocal.TextBox.Carets[i];

            for (int j = commands.Count - 1; j >= 0; j--)
            {
                Command command = commands[j];
                command.Undo(caret);
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
        var sb = new StringBuilder();
        for (int i = 0; i < _caretsCommands.Count; i++)
        {
            sb.AppendLine($"\tCaret {i}: {_caretsCommands[i].Count} commands");
            foreach (var command in _caretsCommands[i])
            {
                sb.AppendLine($"\t\t{command.GetDebugString()}");
            }
        }
        return sb.ToString();
    }
}

internal class DeleteSelection : Action
{
    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();
        if (caret.HasSelection)
            return commands;

        caret.GetSortedSelection(out TextCoordinates start, out TextCoordinates end);

        if (ThreadLocal.TextBox.Lines.Count <= start.LineIndex || ThreadLocal.TextBox.Lines.Count <= end.LineIndex)
            return commands; // TODO assert?

        if (caret.AnchorPosition < caret.Position)
        {
            // Backward delete
            for (int i = caret.Position.LineIndex; i >= caret.AnchorPosition.LineIndex; i--)
            {
                Line lineToDelete = ThreadLocal.TextBox.Lines[i];

                int deleteCount = lineToDelete.Chars.Count;
                if (i == caret.Position.LineIndex)
                {
                    deleteCount = caret.Position.CharIndex;
                }

                if (i == caret.AnchorPosition.LineIndex)
                {
                    deleteCount -= caret.AnchorPosition.CharIndex;
                }

                commands.Add(new DeleteCharCommand(EditDirection.Backward, deleteCount));
                if (i > caret.AnchorPosition.LineIndex)
                {
                    commands.Add(new MergeLineCommand(EditDirection.Backward));
                }
            }
        }
        else if (caret.Position < caret.AnchorPosition)
        {
            // Forward delete
            for (int i = caret.Position.LineIndex; i <= caret.AnchorPosition.LineIndex; i++)
            {
                Line lineToDelete = ThreadLocal.TextBox.Lines[i];

                int deleteCount = lineToDelete.Chars.Count;
                if (i == caret.AnchorPosition.LineIndex)
                {
                    deleteCount = caret.AnchorPosition.CharIndex;
                }

                if (i == caret.Position.LineIndex)
                {
                    deleteCount -= caret.Position.CharIndex;
                }

                commands.Add(new DeleteCharCommand(EditDirection.Forward, deleteCount));
                if (i < caret.AnchorPosition.LineIndex)
                {
                    commands.Add(new MergeLineCommand(EditDirection.Forward));
                }
            }
        }

        return commands;
    }
}

internal class InputCharAction : Action
{
    private readonly EditDirection _direction;
    private readonly char _char;

    public InputCharAction(EditDirection direction, char c)
    {
        _direction = direction;
        _char = c;
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();
        commands.Add(new InputCharCommand(_direction, new[] { _char }));
        return commands;
    }
}

internal class DeleteCharAction : Action
{
    private readonly EditDirection _direction;

    public DeleteCharAction(EditDirection direction)
    {
        _direction = direction;
    }

    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();

        // if caret is at the beginning of the line, merge line (backward)
        if (caret.Position.CharIndex == 0 && _direction == EditDirection.Backward)
        {
            if (caret.Position.LineIndex == 0)
                return commands;

            commands.Add(new MergeLineCommand(EditDirection.Backward));
            return commands;
        }

        // if caret is at the end of the line, merge line (forward)
        if (ThreadLocal.TextBox.Lines.Count > caret.Position.LineIndex)
        {
            var lineToDelete = ThreadLocal.TextBox.Lines[caret.Position.LineIndex];

            if (caret.Position.CharIndex == lineToDelete.Chars.Count - 1 && _direction == EditDirection.Forward)
            {
                if (caret.Position.LineIndex == ThreadLocal.TextBox.Lines.Count - 1)
                    return commands;

                commands.Add(new MergeLineCommand(EditDirection.Forward));
                return commands;
            }
        }

        // else delete char
        commands.Add(new DeleteCharCommand(_direction, 1));
        return commands;
    }
}

internal class EnterAction : Action
{
    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();
        commands.Add(new SplitLineCommand(EditDirection.Forward));
        return commands;
    }
}

internal class TabAction : Action
{
    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();
        if (caret.HasSelection)
        {
            commands.Add(new IndentSelectionCommand());
            return commands;
        }

        commands.Add(new InputCharCommand(EditDirection.Forward, ThreadLocal.TextBox.GetTabString().ToCharArray()));
        return commands;
    }
}

internal class UnTabAction : Action
{
    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();
        commands.Add(new UnindentSelectionCommand());
        return commands;
    }
}