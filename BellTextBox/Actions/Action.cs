﻿using System.Text;
using Bell.Data;
using Bell.Utils;

namespace Bell.Actions;

internal abstract class Action
{
    private readonly List<List<Command>> _caretsCommands = new();

    private readonly List<Caret> _startCarets = new();
    private readonly List<Caret> _endCarets = new();

    private string _startText = "";
    private string _endText = "";
    
    protected abstract List<Command> CreateCommands(Caret caret);

    private void SaveCarets(List<Caret> carets)
    {
        carets.Clear();
        for (int i = 0; i < CaretManager.Count; i++)
        {
            carets.Add(CaretManager.GetCaret(i).Clone());
            Logger.Info("SaveCarets: " + carets[i].Position.LineIndex + " " + carets[i].Position.CharIndex + " " + carets[i].AnchorPosition.LineIndex + " " + carets[i].AnchorPosition.CharIndex);
        }
    }

    private bool RestoreCarets(List<Caret> carets)
    {
        CaretManager.ClearCarets();
        foreach (var caret in carets)
        {
            CaretManager.AddCaret(caret.Clone());
            Logger.Info("RestoreCarets: " + caret.Position.LineIndex + " " + caret.Position.CharIndex + " " + caret.AnchorPosition.LineIndex + " " + caret.AnchorPosition.CharIndex);
        }
        return true;
    }

    internal void DoCommands()
    {
        _caretsCommands.Clear();

        SaveCarets(_startCarets);
        if (Singleton.TextBox.IsDebugMode)
            _startText = Singleton.TextBox.GetText();

        for (int i = 0; i < CaretManager.Count; i++)
        {
            Caret caret = CaretManager.GetCaret(i);
            var commands = CreateCommands(caret);
            _caretsCommands.Add(commands);

            foreach (Command command in commands)
            {
                Logger.Info($"DoCommands: {command.GetDebugString()}");
                command.Do(caret);
            }
        }

        SaveCarets(_endCarets);
        
        if (Singleton.TextBox.IsDebugMode)
            _endText = Singleton.TextBox.GetText();

        if (Singleton.TextBox.IsDebugMode)
        {
            bool isAllSame = true;
            for (int i = 0; i < _endCarets.Count; i++)
            {
                Caret startCaret = _startCarets[i];
                Caret endCaret = _endCarets[i];

                if (CaretManager.CheckValid(startCaret))
                {
                    if (false == startCaret.Position.IsSameAs(endCaret.Position))
                        isAllSame = false;
                    if (false == startCaret.AnchorPosition.IsSameAs(endCaret.AnchorPosition))
                        isAllSame = false;
                }
                else
                {
                    isAllSame = false;
                }
                
                if (false == CaretManager.CheckValid(endCaret))
                {
                    Logger.Error($"DoCommands: invalid end caret: {endCaret.Position.LineIndex} {endCaret.Position.CharIndex} {endCaret.AnchorPosition.LineIndex} {endCaret.AnchorPosition.CharIndex}");
                }
            }

            if (isAllSame)
            {
                Logger.Warning("DoCommands: Carets not changed");
            }
        }
    }

    internal void RedoCommands()
    {
        if (false == RestoreCarets(_startCarets))
        {
            Logger.Error("RedoCommands: failed to restore start carets");
            return;
        }

        if (Singleton.TextBox.IsDebugMode && Singleton.TextBox.GetText() != _startText)
            Logger.Error("RedoCommands: Text not match");

        for (int i = 0; i < _caretsCommands.Count; i++)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = CaretManager.GetCaret(i);

            foreach (Command command in commands)
            {
                Logger.Info($"RedoCommands: {command.GetDebugString()}");
                command.Do(caret);
            }
        }

        RestoreCarets(_endCarets);
            
        if (Singleton.TextBox.IsDebugMode && Singleton.TextBox.GetText() != _endText)
            Logger.Error("RedoCommands: Text not match");
    }

    internal void UndoCommands()
    {
        if (false == RestoreCarets(_endCarets))
        {
            Logger.Error("UndoCommands: failed to restore end carets");
            return;
        }
        
        if (Singleton.TextBox.IsDebugMode && Singleton.TextBox.GetText() != _endText)
            Logger.Error("UndoCommands: Text not match");

        for (int i = _caretsCommands.Count - 1; i >= 0; i--)
        {
            List<Command> commands = _caretsCommands[i];
            Caret caret = CaretManager.GetCaret(i);

            for (int j = commands.Count - 1; j >= 0; j--)
            {
                Command command = commands[j];
                Logger.Info($"UndoCommands: {command.GetDebugString()}");
                command.Undo(caret);
            }
        }

        RestoreCarets(_startCarets);
        
        if (Singleton.TextBox.IsDebugMode && Singleton.TextBox.GetText() != _startText)
            Logger.Error("UndoCommands: Text not match");
    }

    internal bool IsAllSame<T>()
    {
        return _caretsCommands
            .SelectMany(commands => commands)
            .All(command => command is T);
    }

    internal string GetDebugString()
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

internal class DeleteSelectionAction : Action
{
    protected override List<Command> CreateCommands(Caret caret)
    {
        var commands = new List<Command>();
        if (false == caret.HasSelection)
            return commands;

        if (caret.Position.IsBiggerThan(caret.AnchorPosition))
        {
            // Backward delete
            for (int i = caret.Position.LineIndex; i >= caret.AnchorPosition.LineIndex; i--)
            {
                if (!LineManager.GetLine(i, out Line lineToDelete))
                {
                    Logger.Error($"DeleteSelectionAction: failed to get line: {i}");
                    continue;
                }
                
                int deleteCount = lineToDelete.Chars.Count;
                if (i == caret.Position.LineIndex)
                {
                    deleteCount = caret.Position.CharIndex;
                }

                if (i == caret.AnchorPosition.LineIndex)
                {
                    deleteCount -= caret.AnchorPosition.CharIndex;
                }

                if (deleteCount > 0)
                {
                    commands.Add(new DeleteCharCommand(EditDirection.Backward, deleteCount));
                }
                else
                {
                    Logger.Warning("DeleteSelectionAction: deleteCount is 0");
                }
                if (i > caret.AnchorPosition.LineIndex)
                {
                    commands.Add(new MergeLineCommand(EditDirection.Backward));
                }
            }
        }
        if (caret.AnchorPosition.IsBiggerThan(caret.Position))
        {
            // Forward delete
            for (int i = caret.Position.LineIndex; i <= caret.AnchorPosition.LineIndex; i++)
            {
                if (!LineManager.GetLine(i, out Line lineToDelete))
                {
                    Logger.Error($"DeleteSelectionAction: failed to get line: {i}");
                    continue;
                }
                
                int deleteCount = lineToDelete.Chars.Count;
                if (i == caret.AnchorPosition.LineIndex)
                {
                    deleteCount = caret.AnchorPosition.CharIndex;
                }

                if (i == caret.Position.LineIndex)
                {
                    deleteCount -= caret.Position.CharIndex;
                }

                if (deleteCount > 0)
                {
                    commands.Add(new DeleteCharCommand(EditDirection.Forward, deleteCount));
                }
                else
                {
                    Logger.Warning("DeleteSelectionAction: deleteCount is 0");
                }

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

    internal InputCharAction(EditDirection direction, char c)
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

    internal DeleteCharAction(EditDirection direction)
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
        if (false == LineManager.GetLine(caret.Position.LineIndex, out Line line))
        {
            Logger.Error($"DeleteCharAction: failed to get line: {caret.Position.LineIndex}");
            return commands;
        }
        
        if (caret.Position.CharIndex == line.Chars.Count && _direction == EditDirection.Forward)
        {
            if (caret.Position.LineIndex == LineManager.Lines.Count - 1)
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