namespace Bell.Data;

internal abstract class Command
{
    public abstract void Do();
    public abstract void Undo();
}



internal enum EditDirection
{
    Forward,
    Backward
}

internal class InputCharCommand : Command
{
    private EditDirection _direction;
    private char[] _chars;
    
    public InputCharCommand(EditDirection direction, char[] chars)
    {
        _direction = direction;
        _chars = chars;
    }
    
    public override void Do()
    {
    }

    public override void Undo()
    {
    }
}

internal class DeleteCharCommand : Command
{
    private EditDirection _direction;
    private int _count;

    public DeleteCharCommand(EditDirection direction, int count)
    {
        _direction = direction;
        _count = count;
    }
    
    public override void Do()
    {
    }

    public override void Undo()
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
    
    public override void Do()
    {
    }

    public override void Undo()
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
    
    public override void Do()
    {
    }

    public override void Undo()
    {
    }
}

internal class IndentSelection : Command
{
    public override void Do()
    {
    }

    public override void Undo()
    {
    }
}

internal class UnindentSelection : Command
{
    public override void Do()
    {
    }

    public override void Undo()
    {
    }
}