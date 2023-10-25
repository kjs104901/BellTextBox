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
        Singleton.LineManager.RowsCache.SetDirty();

        if (EditDirection.Forward == _direction)
        {
            caret.Position = caret.Position.FindCoordinates(CaretMove.Right);
            caret.RemoveSelection();
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
            Singleton.LineManager.RowsCache.SetDirty();
        }
        else if (EditDirection.Backward == _direction)
        {
            if (targetIndex - _deletedCount < 0)
                _deletedCount = targetIndex;
                
            _deletedChars = chars.GetRange(targetIndex - _deletedCount, _deletedCount).ToArray();
            chars.RemoveRange(targetIndex - _deletedCount, _deletedCount);
            line.SetCharsDirty();
            Singleton.LineManager.RowsCache.SetDirty();

            caret.Position = caret.Position.FindCoordinates(CaretMove.Left);
            caret.RemoveSelection();
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
            Line newLine = Singleton.LineManager.InsertLine(insertLineIndex, restOfLine);
            
            caret.Position = new LineCoordinates() { Line = newLine, CharIndex = 0 };
            caret.RemoveSelection();
        }
        else
        {
            // Get backward rest of line
            restOfLine = chars.GetRange(0, caret.Position.CharIndex).ToArray();
            chars.RemoveRange(0, caret.Position.CharIndex);
            line.SetCharsDirty();
            
            insertLineIndex = caret.Position.Line.Index;
            Line newLine = Singleton.LineManager.InsertLine(insertLineIndex, restOfLine);
            
            caret.Position = new LineCoordinates() { Line = newLine, CharIndex = restOfLine.Length };
            caret.RemoveSelection();
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
        Line line = caret.Position.Line;
        
        if (EditDirection.Forward == _direction)
        {
            int nextLineIndex = line.Index + 1;
            
            if (false == Singleton.LineManager.GetLine(nextLineIndex, out Line nextLine))
                return;
                
            line.Chars.AddRange(nextLine.Chars);
            line.SetCharsDirty();

            // TODO 정리?
            foreach (Caret moveCaret in Singleton.CaretManager.GetCaretsInLine(nextLine))
            {
                moveCaret.Position.Line = line;
                moveCaret.Position.CharIndex += line.Chars.Count;
                moveCaret.AnchorPosition.Line = line;
                moveCaret.AnchorPosition.CharIndex += line.Chars.Count;
            }
            
            Singleton.LineManager.RemoveLine(nextLineIndex);
        }
        else if (EditDirection.Backward == _direction)
        {
            int currentLineIndex = line.Index;
            int prevLineIndex = line.Index - 1;
            
            if (false == Singleton.LineManager.GetLine(prevLineIndex, out Line prevLine))
                return;
            
            foreach (Caret moveCaret in Singleton.CaretManager.GetCaretsInLine(line))
            {
                moveCaret.Position.Line = prevLine;
                moveCaret.Position.CharIndex += prevLine.Chars.Count;
                moveCaret.AnchorPosition.Line = prevLine;
                moveCaret.AnchorPosition.CharIndex += prevLine.Chars.Count;
            }
                
            prevLine.Chars.AddRange(line.Chars);
            prevLine.SetCharsDirty();
            
            Singleton.LineManager.RemoveLine(currentLineIndex);
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