namespace Bell.Data;

internal abstract class Command
{
    public abstract void Do(Caret caret);
    public abstract void Undo(Caret caret);
    public abstract string GetDebugString();
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

    public override void Do(Caret caret)
    {
    }

    public override void Undo(Caret caret)
    {
    }

    public override string GetDebugString()
    {
        throw new NotImplementedException();
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

    public override void Do(Caret caret)
    {
    }

    public override void Undo(Caret caret)
    {
    }

    public override string GetDebugString()
    {
        throw new NotImplementedException();
    }
}

internal class SplitLineCommand : Command
{
    private EditDirection _direction;

    public SplitLineCommand(EditDirection direction)
    {
        _direction = direction;
    }

    public override void Do(Caret caret)
    {
    }

    public override void Undo(Caret caret)
    {
    }

    public override string GetDebugString()
    {
        throw new NotImplementedException();
    }
}

internal class MergeLineCommand : Command
{
    private EditDirection _direction;

    public MergeLineCommand(EditDirection direction)
    {
        _direction = direction;
    }

    public override void Do(Caret caret)
    {
    }

    public override void Undo(Caret caret)
    {
    }

    public override string GetDebugString()
    {
        throw new NotImplementedException();
    }
}

internal class IndentSelectionCommand : Command
{
    public override void Do(Caret caret)
    {
    }

    public override void Undo(Caret caret)
    {
    }

    public override string GetDebugString()
    {
        throw new NotImplementedException();
    }
}

internal class UnindentSelectionCommand : Command
{
    public override void Do(Caret caret)
    {
    }

    public override void Undo(Caret caret)
    {
    }

    public override string GetDebugString()
    {
        throw new NotImplementedException();
    }
}