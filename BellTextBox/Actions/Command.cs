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
        if (ThreadLocal.TextBox.GetLine(caret.Position.LineIndex, out Line line))
        {
            int targetIndex = caret.Position.CharIndex;
            if (targetIndex < 0 || targetIndex > line.Chars.Count)
            {
                // TODO error
                return;
            }
            
            line.Chars.InsertRange(targetIndex, _chars);
            line.SetCharsDirty();
            ThreadLocal.TextBox.RowsCache.SetDirty();

            if (EditDirection.Forward == _direction)
            {
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.Right);
                ThreadLocal.TextBox.RemoveCaretsSelection();
            }
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
        if (ThreadLocal.TextBox.GetLine(caret.Position.LineIndex, out Line line))
        {
            _deletedCount = _count;
            
            int targetIndex = caret.Position.CharIndex;
            if (targetIndex < 0 || targetIndex > line.Chars.Count)
            {
                // TODO error
                return;
            }
            
            if (EditDirection.Forward == _direction)
            {
                if (targetIndex + _deletedCount > line.Chars.Count)
                    _deletedCount = line.Chars.Count - targetIndex;

                _deletedChars = line.Chars.GetRange(targetIndex, _deletedCount).ToArray();
                line.Chars.RemoveRange(targetIndex, _deletedCount);
                line.SetCharsDirty();
                ThreadLocal.TextBox.RowsCache.SetDirty();
            }
            else if (EditDirection.Backward == _direction)
            {
                if (targetIndex - _deletedCount < 0)
                    _deletedCount = targetIndex;
                
                _deletedChars = line.Chars.GetRange(targetIndex - _deletedCount, _deletedCount).ToArray();
                line.Chars.RemoveRange(targetIndex - _deletedCount, _deletedCount);
                line.SetCharsDirty();
                ThreadLocal.TextBox.RowsCache.SetDirty();

                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.Left);
                ThreadLocal.TextBox.RemoveCaretsSelection();
            }
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
        if (ThreadLocal.TextBox.GetLine(caret.Position.LineIndex, out Line line))
        {
            char[] restOfLine;
            int insertLineIndex;
            
            if (EditDirection.Forward == _direction)
            {
                // Get forward rest of line
                restOfLine = line.Chars.GetRange(caret.Position.CharIndex, line.Chars.Count - caret.Position.CharIndex).ToArray();
                line.Chars.RemoveRange(caret.Position.CharIndex, line.Chars.Count - caret.Position.CharIndex);
                line.SetCharsDirty();
                
                // TODO auto indent?
                
                insertLineIndex = caret.Position.LineIndex + 1;
                
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.Down);
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.StartOfLine);
                ThreadLocal.TextBox.RemoveCaretsSelection();
            }
            else
            {
                // Get backward rest of line
                restOfLine = line.Chars.GetRange(0, caret.Position.CharIndex).ToArray();
                line.Chars.RemoveRange(0, caret.Position.CharIndex);
                line.SetCharsDirty();
                
                insertLineIndex = caret.Position.LineIndex;
                    
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.Up);
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.EndOfLine);
                ThreadLocal.TextBox.RemoveCaretsSelection();
            }
            
            // Update line index
            foreach (Line textBoxLine in ThreadLocal.TextBox.Lines)
            {
                if (textBoxLine.Index >= insertLineIndex)
                {
                    textBoxLine.Index++;
                    textBoxLine.SetCharsDirty();
                }
            }
                
            // Create new line and insert
            Line newLine = new Line(insertLineIndex, restOfLine);
                
            ThreadLocal.TextBox.Lines.Insert(insertLineIndex, newLine);
            ThreadLocal.TextBox.RowsCache.SetDirty();
        }
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
        if (ThreadLocal.TextBox.GetLine(caret.Position.LineIndex, out Line line))
        {
            if (EditDirection.Forward == _direction)
            {
                if (false == ThreadLocal.TextBox.GetLine(caret.Position.LineIndex + 1, out Line nextLine))
                    return;
                
                line.Chars.AddRange(nextLine.Chars);
                line.SetCharsDirty();
                
                ThreadLocal.TextBox.Lines.RemoveAt(caret.Position.LineIndex + 1);
                ThreadLocal.TextBox.RowsCache.SetDirty();
            }
            else if (EditDirection.Backward == _direction)
            {
                if (false == ThreadLocal.TextBox.GetLine(caret.Position.LineIndex - 1, out Line prevLine))
                    return;
                
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.Up);
                ThreadLocal.TextBox.MoveCaretsPosition(CaretMove.EndOfLine);
                ThreadLocal.TextBox.RemoveCaretsSelection();
                
                prevLine.Chars.AddRange(line.Chars);
                prevLine.SetCharsDirty();
                
                ThreadLocal.TextBox.Lines.RemoveAt(caret.Position.LineIndex);
                ThreadLocal.TextBox.RowsCache.SetDirty();
            }
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