namespace Bell.Commands;

internal enum EditDirection
{
    Forward,
    Backward
}

internal class InputChar : EditCommand
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

internal class DeleteCommand : EditCommand
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

internal class SplitLine : EditCommand
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

internal class MergeLine : EditCommand
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

internal class IndentSelection : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class UnindentSelection : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class CopyCommand : Command
{
    public override void Do(TextBox textBox)
    {
    }
}

internal class PasteCommand : Command
{
    public override void Do(TextBox textBox)
    {
        //ClipboardText
    }
}