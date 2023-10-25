using Bell.Data;
using Bell.Utils;

namespace Bell.Actions;

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
    private readonly EditDirection _direction;
    private readonly char[] _chars;

    public InputCharCommand(EditDirection direction, char[] chars)
    {
        _direction = direction;
        _chars = chars;
    }

    public override void Do(Caret caret)
    {
        Line line = caret.Position.Line;
        var chars = line.Chars;
        
        int targetIndex = caret.Position.CharIndex;
        if (targetIndex < 0 || targetIndex > chars.Count)
        {
            // TODO error
            return;
        }
            
        chars.InsertRange(targetIndex, _chars);
        line.SetCharsDirty();
        ThreadLocal.LineManager.RowsCache.SetDirty();

        if (EditDirection.Forward == _direction)
        {
            ThreadLocal.CaretManager.MoveCaretsPosition(CaretMove.Right);
            ThreadLocal.CaretManager.RemoveCaretsSelection();
        }
    }

    public override void Undo(Caret caret)
    {
        if (EditDirection.Forward == _direction)
            new DeleteCharCommand(EditDirection.Backward, _chars.Length).Do(caret);
        else if (EditDirection.Backward == _direction)
            new DeleteCharCommand(EditDirection.Forward, _chars.Length).Do(caret);
    }

    public override string GetDebugString()
    {
        return string.Join(' ', _chars);
    }
}

internal class DeleteCharCommand : Command
{
    private readonly EditDirection _direction;
    private readonly int _count;
    
    private int _deletedCount;
    private char[] _deletedChars = Array.Empty<char>();

    public DeleteCharCommand(EditDirection direction, int count)
    {
        _direction = direction;
        _count = count;
    }

    public override void Do(Caret caret)
    {
        Line line = caret.Position.Line;
        List<char> chars = line.Chars;
        
        _deletedCount = _count;
            
        int targetIndex = caret.Position.CharIndex;
        if (targetIndex < 0 || targetIndex > chars.Count)
        {
            // TODO error
            return;
        }
            
        if (EditDirection.Forward == _direction)
        {
            if (targetIndex + _deletedCount > chars.Count)
                _deletedCount = chars.Count - targetIndex;

            _deletedChars = chars.GetRange(targetIndex, _deletedCount).ToArray();
            chars.RemoveRange(targetIndex, _deletedCount);
            line.SetCharsDirty();
            ThreadLocal.LineManager.RowsCache.SetDirty();
        }
        else if (EditDirection.Backward == _direction)
        {
            if (targetIndex - _deletedCount < 0)
                _deletedCount = targetIndex;
                
            _deletedChars = chars.GetRange(targetIndex - _deletedCount, _deletedCount).ToArray();
            chars.RemoveRange(targetIndex - _deletedCount, _deletedCount);
            line.SetCharsDirty();
            ThreadLocal.LineManager.RowsCache.SetDirty();

            ThreadLocal.CaretManager.MoveCaretsPosition(CaretMove.Left);
            ThreadLocal.CaretManager.RemoveCaretsSelection();
        }
    }

    public override void Undo(Caret caret)
    {
        if (EditDirection.Forward == _direction)
            new InputCharCommand(EditDirection.Backward, _deletedChars).Do(caret);
        else if (EditDirection.Backward == _direction)
            new InputCharCommand(EditDirection.Forward, _deletedChars).Do(caret);
    }

    public override string GetDebugString()
    {
        return $"Delete Char {_count} {_direction}";
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
        char[] restOfLine;
        int insertLineIndex;

        Line line = caret.Position.Line;
        List<char> chars = line.Chars;
        
        if (EditDirection.Forward == _direction)
        {
            // Get forward rest of line
            restOfLine = chars.GetRange(caret.Position.CharIndex, chars.Count - caret.Position.CharIndex).ToArray();
            chars.RemoveRange(caret.Position.CharIndex, chars.Count - caret.Position.CharIndex);
            line.SetCharsDirty();
            
            // TODO auto indent?
            
            insertLineIndex = caret.Position.Line.Index + 1;
            
            ThreadLocal.CaretManager.MoveCaretsPosition(CaretMove.Down);
            ThreadLocal.CaretManager.MoveCaretsPosition(CaretMove.StartOfLine);
            ThreadLocal.CaretManager.RemoveCaretsSelection();
        }
        else
        {
            // Get backward rest of line
            restOfLine = chars.GetRange(0, caret.Position.CharIndex).ToArray();
            chars.RemoveRange(0, caret.Position.CharIndex);
            line.SetCharsDirty();
            
            insertLineIndex = caret.Position.Line.Index;
                
            ThreadLocal.CaretManager.MoveCaretsPosition(CaretMove.Up);
            ThreadLocal.CaretManager.MoveCaretsPosition(CaretMove.EndOfLine);
            ThreadLocal.CaretManager.RemoveCaretsSelection();
        }
        
        // Create new line and insert
        ThreadLocal.LineManager.InsertLine(insertLineIndex, restOfLine);
    }

    public override void Undo(Caret caret)
    {
        // TODO auto indent?
        
        if (EditDirection.Forward == _direction)
            new MergeLineCommand(EditDirection.Backward).Do(caret);
        else if (EditDirection.Backward == _direction)
            new MergeLineCommand(EditDirection.Forward).Do(caret);
    }

    public override string GetDebugString()
    {
        return $"Split Line {_direction}";
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
        Line line = caret.Position.Line;
        List<char> chars = line.Chars;
        
        if (EditDirection.Forward == _direction)
        {
            int removeLineIndex = caret.Position.Line.Index + 1;
            
            if (false == ThreadLocal.LineManager.GetLine(removeLineIndex, out Line nextLine))
                return;
                
            chars.AddRange(nextLine.Chars);
            line.SetCharsDirty();
            
            ThreadLocal.LineManager.RemoveLine(removeLineIndex);
        }
        else if (EditDirection.Backward == _direction)
        {
            int removeLineIndex = caret.Position.Line.Index - 1;
            
            if (false == ThreadLocal.LineManager.GetLine(removeLineIndex, out Line prevLine))
                return;
                
            caret.Position.Line = prevLine;
            caret.Position.CharIndex = prevLine.Chars.Count;
            caret.AnchorPosition.Line = prevLine;
            caret.AnchorPosition.CharIndex = prevLine.Chars.Count;
                
            prevLine.Chars.AddRange(chars);
            prevLine.SetCharsDirty();
            
            ThreadLocal.LineManager.RemoveLine(caret.Position.Line.Index);
        }
    }

    public override void Undo(Caret caret)
    {
        if (EditDirection.Forward == _direction)
            new SplitLineCommand(EditDirection.Backward).Do(caret);
        else if (EditDirection.Backward == _direction)
            new SplitLineCommand(EditDirection.Forward).Do(caret);
    }

    public override string GetDebugString()
    {
        return $"Merge Line {_direction}";
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
        return "Indent Selection";
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
        return "Unindent Selection";
    }
}