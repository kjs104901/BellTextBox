﻿using System.Diagnostics;
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
        string result = "";
        for (int i = 0; i < _caretsCommands.Count; i++)
        {
            result += $"\tCaret {i}: {_caretsCommands[i].Count} commands\n";

            foreach (var command in _caretsCommands[i])
            {
                result += $"\t\t{command.GetDebugString()}\n";
            }
        }
        return result;
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
        //if (ThreadLocal.TextBox.Rows.Count <= start.Row || ThreadLocal.TextBox.Rows.Count <= end.Row)
        //    return commands;
        //
        //SubLine startLine = ThreadLocal.TextBox.Rows[start.Row];
        //SubLine endLine = ThreadLocal.TextBox.Rows[end.Row];

        //startLine.LineIndex;
        
        
        //commands.Add(new DeleteCharCommand(EditDirection.Forward, ));
        
        return commands;
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