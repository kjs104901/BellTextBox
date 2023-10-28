﻿using Bell.Data;
using Bell.Utils;

namespace Bell.Actions;

internal abstract class Command
{
    internal abstract void Do(Caret caret);
    internal abstract void Undo(Caret caret);
    internal abstract string GetDebugString();
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

    internal InputCharCommand(EditDirection direction, char[] chars)
    {
        _direction = direction;
        _chars = chars;
    }

    internal override void Do(Caret caret)
    {
        if (false == LineManager.GetLine(caret.Position.LineIndex, out Line line))
        {
            Logger.Error($"InputCharCommand: Line not found {caret.Position.LineIndex}");
            return;
        }
        // TODO 같은 줄 캐럿 이동
        line.Chars.InsertRange(caret.Position.CharIndex, _chars);
        line.SetCharsDirty();
        RowManager.SetRowCacheDirty();

        if (EditDirection.Forward == _direction)
        {
            caret.Position = caret.Position.FindMove(CaretMove.CharRight, _chars.Length);
        }
        caret.RemoveSelection();
        
        FoldingManager.SetCacheDirty();
    }

    internal override void Undo(Caret caret)
    {
        if (EditDirection.Forward == _direction)
            new DeleteCharCommand(EditDirection.Backward, _chars.Length).Do(caret);
        else if (EditDirection.Backward == _direction)
            new DeleteCharCommand(EditDirection.Forward, _chars.Length).Do(caret);
    }

    internal override string GetDebugString()
    {
        return $"Input Char {string.Join(' ', _chars)} {_direction}";
    }
}

internal class DeleteCharCommand : Command
{
    private readonly EditDirection _direction;
    private readonly int _count;
    
    private int _deletedCount;
    private char[] _deletedChars = Array.Empty<char>();

    internal DeleteCharCommand(EditDirection direction, int count)
    {
        _direction = direction;
        _count = count;
    }

    internal override void Do(Caret caret)
    {
        if (false == LineManager.GetLine(caret.Position.LineIndex, out Line line))
        {
            Logger.Error($"DeleteCharCommand: Line not found {caret.Position.LineIndex}");
            return;
        }

        _deletedCount = _count;
            
        List<char> chars = line.Chars;
        int targetIndex = caret.Position.CharIndex;
            
        if (EditDirection.Forward == _direction)
        {
            if (targetIndex + _deletedCount > chars.Count)
                _deletedCount = chars.Count - targetIndex;

            _deletedChars = chars.GetRange(targetIndex, _deletedCount).ToArray();
            // TODO 같은 줄 캐럿 이동
            chars.RemoveRange(targetIndex, _deletedCount);
            line.SetCharsDirty();
            RowManager.SetRowCacheDirty();
            
            caret.RemoveSelection();
        }
        else if (EditDirection.Backward == _direction)
        {
            if (targetIndex - _deletedCount < 0)
                _deletedCount = targetIndex;
                
            _deletedChars = chars.GetRange(targetIndex - _deletedCount, _deletedCount).ToArray();
            chars.RemoveRange(targetIndex - _deletedCount, _deletedCount);
            line.SetCharsDirty();
            RowManager.SetRowCacheDirty();

            caret.Position = caret.Position.FindMove(CaretMove.CharLeft, _deletedCount);
            caret.RemoveSelection();
        }

        if (_count != _deletedCount)
        {
            Logger.Error($"DeleteCharCommand: _count != _deletedCount {_count} {_deletedCount}");
        }
        FoldingManager.SetCacheDirty();
    }

    internal override void Undo(Caret caret)
    {
        if (EditDirection.Forward == _direction)
            new InputCharCommand(EditDirection.Backward, _deletedChars).Do(caret);
        else if (EditDirection.Backward == _direction)
            new InputCharCommand(EditDirection.Forward, _deletedChars).Do(caret);
    }

    internal override string GetDebugString()
    {
        return $"Delete Char {_count} {_direction}";
    }
}

internal class SplitLineCommand : Command
{
    private EditDirection _direction;

    internal SplitLineCommand(EditDirection direction)
    {
        _direction = direction;
    }

    internal override void Do(Caret caret)
    {
        if (false == LineManager.GetLine(caret.Position.LineIndex, out Line line))
        {
            Logger.Error($"SplitLineCommand: Line not found {caret.Position.LineIndex}");
            return;
        }
        
        char[] restOfLine;
        int insertLineIndex;

        List<char> chars = line.Chars;
        
        if (EditDirection.Forward == _direction)
        {
            // Get forward rest of line
            restOfLine = chars.GetRange(caret.Position.CharIndex, chars.Count - caret.Position.CharIndex).ToArray();
            chars.RemoveRange(caret.Position.CharIndex, chars.Count - caret.Position.CharIndex);
            line.SetCharsDirty();
            
            // TODO auto indent?
            
            // TODO 캐럿 이동
            
            insertLineIndex = caret.Position.LineIndex + 1;
            Line newLine = LineManager.InsertLine(insertLineIndex, restOfLine);
            
            caret.Position = new Coordinates(insertLineIndex, 0);
            caret.RemoveSelection();
        }
        else
        {
            // Get backward rest of line
            restOfLine = chars.GetRange(0, caret.Position.CharIndex).ToArray();
            chars.RemoveRange(0, caret.Position.CharIndex);
            line.SetCharsDirty();
            
            // TODO 캐럿 이동
            
            insertLineIndex = caret.Position.LineIndex;
            Line newLine = LineManager.InsertLine(insertLineIndex, restOfLine);

            int charIndex = restOfLine.Length;
            caret.Position = new Coordinates(insertLineIndex, charIndex);
            caret.RemoveSelection();
        }
        FoldingManager.SetCacheDirty();
    }

    internal override void Undo(Caret caret)
    {
        // TODO auto indent?
        
        if (EditDirection.Forward == _direction)
            new MergeLineCommand(EditDirection.Backward).Do(caret);
        else if (EditDirection.Backward == _direction)
            new MergeLineCommand(EditDirection.Forward).Do(caret);
    }

    internal override string GetDebugString()
    {
        return $"Split Line {_direction}";
    }
}

internal class MergeLineCommand : Command
{
    private EditDirection _direction;

    internal MergeLineCommand(EditDirection direction)
    {
        _direction = direction;
    }

    internal override void Do(Caret caret)
    {
        if (false == LineManager.GetLine(caret.Position.LineIndex, out Line line))
        {
            Logger.Error($"MergeLineCommand: Line not found {caret.Position.LineIndex}");
            return;
        }
        
        if (EditDirection.Forward == _direction)
        {
            int nextLineIndex = line.Index + 1;
            
            if (false == LineManager.GetLine(nextLineIndex, out Line nextLine))
                return;
                
            line.Chars.AddRange(nextLine.Chars);
            line.SetCharsDirty();

            // TODO 정리?
            foreach (Caret moveCaret in CaretManager.GetCaretsInLine(nextLine))
            {
                moveCaret.Position.LineIndex = line.Index;
                moveCaret.Position.CharIndex += line.Chars.Count;
                moveCaret.AnchorPosition.LineIndex = line.Index;
                moveCaret.AnchorPosition.CharIndex += line.Chars.Count;
            }
            
            LineManager.RemoveLine(nextLineIndex);
        }
        else if (EditDirection.Backward == _direction)
        {
            int currentLineIndex = line.Index;
            int prevLineIndex = line.Index - 1;
            
            if (false == LineManager.GetLine(prevLineIndex, out Line prevLine))
                return;
            
            foreach (Caret moveCaret in CaretManager.GetCaretsInLine(line))
            {
                moveCaret.Position.LineIndex = prevLine.Index;
                moveCaret.Position.CharIndex += prevLine.Chars.Count;
                moveCaret.AnchorPosition.LineIndex = prevLine.Index;
                moveCaret.AnchorPosition.CharIndex += prevLine.Chars.Count;
            }
                
            prevLine.Chars.AddRange(line.Chars);
            prevLine.SetCharsDirty();
            
            LineManager.RemoveLine(currentLineIndex);
        }
        FoldingManager.SetCacheDirty();
    }

    internal override void Undo(Caret caret)
    {
        if (EditDirection.Forward == _direction)
            new SplitLineCommand(EditDirection.Backward).Do(caret);
        else if (EditDirection.Backward == _direction)
            new SplitLineCommand(EditDirection.Forward).Do(caret);
    }

    internal override string GetDebugString()
    {
        return $"Merge Line {_direction}";
    }
}

internal class IndentSelectionCommand : Command
{
    internal override void Do(Caret caret)
    {
    }

    internal override void Undo(Caret caret)
    {
    }

    internal override string GetDebugString()
    {
        return "Indent Selection";
    }
}

internal class UnindentSelectionCommand : Command
{
    internal override void Do(Caret caret)
    {
    }

    internal override void Undo(Caret caret)
    {
    }

    internal override string GetDebugString()
    {
        return "Unindent Selection";
    }
}