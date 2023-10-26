using System.Text;
using Bell.Data;
using Bell.Utils;

namespace Bell.Actions;

internal abstract class Action
{
    private readonly List<List<Command>> _caretsCommands = new();

    private readonly List<CaretCoordinates> _startCarets = new();
    private readonly List<CaretCoordinates> _endCarets = new();

    protected abstract List<Command> CreateCommands(Caret caret);

    private void SaveCarets(List<CaretCoordinates> coordinatesList)
    {
        coordinatesList.Clear();
        foreach (Caret caret in Singleton.CaretManager.Carets)
        {
            coordinatesList.Add(caret.GetCaretCoordinates());
        }
    }
    
    private void RestoreCarets(List<CaretCoordinates> coordinatesList)
    {
        Singleton.CaretManager.Carets.Clear();
        foreach (CaretCoordinates caretCoordinates in coordinatesList)
        {
            if (false == Singleton.LineManager.GetLine(caretCoordinates.PositionLineIndex, out Line line))
            {
                Singleton.Logger.Error($"RestoreStartCarets: line not found: {caretCoordinates.PositionLineIndex}");
                continue;
            }

            if (line.Chars.Count < caretCoordinates.PositionCharIndex)
            {
                Singleton.Logger.Error($"RestoreStartCarets: char not found: {caretCoordinates.PositionCharIndex}");
                continue;
            }

            if (false == Singleton.LineManager.GetLine(caretCoordinates.AnchorPositionLineIndex, out Line anchorLine))
            {
                Singleton.Logger.Error($"RestoreStartCarets: anchor line not found: {caretCoordinates.AnchorPositionLineIndex}");
                continue;
            }
            
            if (anchorLine.Chars.Count < caretCoordinates.AnchorPositionCharIndex)
            {
                Singleton.Logger.Error($"RestoreStartCarets: anchor char not found: {caretCoordinates.PositionCharIndex}");
                continue;
            }
            
            Singleton.CaretManager.Carets.Add(new Caret()
            {
                Position = new LineCoordinates() { Line = line, CharIndex = caretCoordinates.PositionCharIndex },
                AnchorPosition = new LineCoordinates() { Line = anchorLine, CharIndex = caretCoordinates.AnchorPositionCharIndex },
            });
        }
        Singleton.CaretManager.SetCaretDirty();
    }

    public void DoCommands()
    {
        _caretsCommands.Clear();

        SaveCarets(_startCarets);

        foreach (Caret caret in Singleton.CaretManager.Carets)
        {
            var commands = CreateCommands(caret);
            _caretsCommands.Add(commands);

            foreach (Command command in commands)
            {
                command.Do(caret);
                
                Singleton.Logger.Info($"DoCommands: {command.GetDebugString()}");
            }
        }
        SaveCarets(_endCarets);
    }
    
    public void RedoCommands()
    {
        RestoreCarets(_startCarets);
        
        for (int i = 0; i < _caretsCommands.Count; i++)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = Singleton.CaretManager.Carets[i];

            foreach (Command command in commands)
            {
                command.Do(caret);
                
                Singleton.Logger.Info($"RedoCommands: {command.GetDebugString()}");
            }
        }

        RestoreCarets(_endCarets);
    }

    public void UndoCommands()
    {
        RestoreCarets(_endCarets);

        for (int i = _caretsCommands.Count - 1; i >= 0; i--)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = Singleton.CaretManager.Carets[i];

            for (int j = commands.Count - 1; j >= 0; j--)
            {
                Command command = commands[j];
                command.Undo(caret);
                
                Singleton.Logger.Info($"UndoCommands: {command.GetDebugString()}");
            }
        }

        RestoreCarets(_startCarets);
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
        if (false == caret.HasSelection)
            return commands;

        caret.GetSortedSelection(out LineCoordinates start, out LineCoordinates end);

        if (Singleton.LineManager.Lines.Count <= start.Line.Index || Singleton.LineManager.Lines.Count <= end.Line.Index)
            return commands; // TODO assert?

        if (caret.AnchorPosition < caret.Position)
        {
            // Backward delete
            for (int i = caret.Position.Line.Index; i >= caret.AnchorPosition.Line.Index; i--)
            {
                if (Singleton.LineManager.GetLine(i, out Line lineToDelete))
                {
                    int deleteCount = lineToDelete.Chars.Count;
                    if (i == caret.Position.Line.Index)
                    {
                        deleteCount = caret.Position.CharIndex;
                    }

                    if (i == caret.AnchorPosition.Line.Index)
                    {
                        deleteCount -= caret.AnchorPosition.CharIndex;
                    }

                    commands.Add(new DeleteCharCommand(EditDirection.Backward, deleteCount));
                    if (i > caret.AnchorPosition.Line.Index)
                    {
                        commands.Add(new MergeLineCommand(EditDirection.Backward));
                    }
                }
            }
        }
        else if (caret.Position < caret.AnchorPosition)
        {
            // Forward delete
            for (int i = caret.Position.Line.Index; i <= caret.AnchorPosition.Line.Index; i++)
            {
                if (Singleton.LineManager.GetLine(i, out Line lineToDelete))
                {
                    int deleteCount = lineToDelete.Chars.Count;
                    if (i == caret.AnchorPosition.Line.Index)
                    {
                        deleteCount = caret.AnchorPosition.CharIndex;
                    }

                    if (i == caret.Position.Line.Index)
                    {
                        deleteCount -= caret.Position.CharIndex;
                    }

                    commands.Add(new DeleteCharCommand(EditDirection.Forward, deleteCount));
                    if (i < caret.AnchorPosition.Line.Index)
                    {
                        commands.Add(new MergeLineCommand(EditDirection.Forward));
                    }
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
            if (caret.Position.Line.Index == 0)
                return commands;

            commands.Add(new MergeLineCommand(EditDirection.Backward));
            return commands;
        }

        // if caret is at the end of the line, merge line (forward)
        if (caret.Position.CharIndex == caret.Position.Line.Chars.Count && _direction == EditDirection.Forward)
        {
            if (caret.Position.Line.Index == Singleton.LineManager.Lines.Count - 1)
                return commands;

            commands.Add(new MergeLineCommand(EditDirection.Forward));
            return commands;
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

        commands.Add(new InputCharCommand(EditDirection.Forward, Singleton.TextBox.GetTabString().ToCharArray()));
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