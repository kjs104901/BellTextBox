﻿namespace Bell.Actions;

internal enum EditDirection
{
    Forward,
    Backward
}

internal class InputChar : Command
{
    private EditDirection _direction;
    private char[] _chars;
    
    public InputChar(EditDirection direction, char[] chars)
    {
        _direction = direction;
        _chars = chars;
    }
    
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class DeleteCommand : Command
{
    private EditDirection _direction;
    private int _count;

    public DeleteCommand(EditDirection direction, int count)
    {
        _direction = direction;
        _count = count;
    }
    
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class SplitLine : Command
{
    private EditDirection _direction;
    
    public SplitLine(EditDirection direction)
    {
        _direction = direction;
    }
    
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class MergeLine : Command
{
    private EditDirection _direction;
    
    public MergeLine(EditDirection direction)
    {
        _direction = direction;
    }
    
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class IndentSelection : Command
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class UnindentSelection : Command
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}