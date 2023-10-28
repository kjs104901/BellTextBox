using Bell.Utils;

namespace Bell.Data;

public struct Coordinates
{
    public int LineIndex;
    public int CharIndex;
    
    public int LineSubIndex;

    public Coordinates(int lineIndex, int charIndex, int lineSubIndex = -1)
    {
        LineIndex = lineIndex;
        CharIndex = charIndex;
        LineSubIndex = lineSubIndex;
    }

    public bool IsSameAs(Coordinates other)
    {
        if (false == Singleton.LineManager.GetSubLine(this, out LineSub lineSub) ||
            false == Singleton.LineManager.GetSubLine(other,  out LineSub otherLineSub))
        {
            Logger.Error($"IsSameAs: failed to get line: {LineIndex} or {other.LineIndex}");
            return false;
        }
        
        if (LineIndex != other.LineIndex)
            return false;
            
        if (lineSub.Coordinates.LineSubIndex != otherLineSub.Coordinates.LineSubIndex)
            return false;

        return CharIndex == other.CharIndex;
    }

    public bool IsBiggerThan(Coordinates other)
    {
        if (false == Singleton.LineManager.GetSubLine(this,  out LineSub lineSub) ||
            false == Singleton.LineManager.GetSubLine(other,  out LineSub otherLineSub))
        {
            Logger.Error($"IsBiggerThan: failed to get line: {LineIndex} or {other.LineIndex}");
            return false;
        }
        
        if (LineIndex != other.LineIndex)
            return LineIndex > other.LineIndex;

        if (lineSub.Coordinates.LineSubIndex != otherLineSub.Coordinates.LineSubIndex)
            return lineSub.Coordinates.LineSubIndex > otherLineSub.Coordinates.LineSubIndex;
            
        return CharIndex > other.CharIndex;
    }

    public bool IsValid()
    {
        if (false == Singleton.LineManager.GetLine(LineIndex, out Line line))
            return false;

        if (CharIndex < 0 || CharIndex > line.Chars.Count)
            return false;

        return true;
    }
    
    public Coordinates FindMove(CaretMove caretMove, int count = 1)
    {
        Coordinates newCoordinates = this;
        for (int i = 0; i < count; i++)
        {
            newCoordinates = newCoordinates.FindMoveSingle(caretMove);
        }
        
        Logger.Info("FindMove: " + caretMove + " " + count + " " + newCoordinates.LineIndex + " " + newCoordinates.CharIndex);

        if (IsValid())
        {
            if (IsSameAs(newCoordinates))
            {
                Logger.Warning($"FindMove: IsSameAs {caretMove} {count}");
            }
        }
        return newCoordinates;
    }

    private Coordinates FindMoveSingle(CaretMove caretMove)
    {
        if (CaretMove.Right == caretMove)
        {
            if (Singleton.LineManager.GetLine(LineIndex, out Line line))
            {
                // Check end of line
                if (CharIndex < line.Chars.Count)
                {
                    CharIndex++;
                }
                else
                {
                    // Check end of file
                    if (Singleton.LineManager.GetLine(LineIndex + 1, out Line nextLine))
                    {
                        LineIndex = nextLine.Index;
                        CharIndex = 0;
                    }
                }
            }
        }
        else if (CaretMove.Left == caretMove)
        {
            // Check start of line
            if (CharIndex > 0)
            {
                CharIndex--;
            }
            else
            {
                // Check start of file
                if (Singleton.LineManager.GetLine(LineIndex - 1, out Line prevLine))
                {
                    LineIndex = prevLine.Index;
                    CharIndex = prevLine.Chars.Count;
                }
            }
        }
        else if (CaretMove.Up == caretMove)
        {
            // Check start of file
            if (Singleton.LineManager.GetLine(LineIndex - 1, out Line prevLine))
            {
                LineIndex = prevLine.Index;
                CharIndex = Math.Min(CharIndex, prevLine.Chars.Count);
            }
        }
        else if (CaretMove.Down == caretMove)
        {
            // Check end of file
            if (Singleton.LineManager.GetLine(LineIndex + 1, out Line nextLine))
            {
                LineIndex = nextLine.Index;
                CharIndex = Math.Min(CharIndex, nextLine.Chars.Count);
            }
        }
        else if (CaretMove.StartOfLine == caretMove)
        {
            CharIndex = 0;
        }
        else if (CaretMove.EndOfLine == caretMove)
        {
            if (Singleton.LineManager.GetLine(LineIndex, out Line line))
            {
                CharIndex = line.Chars.Count;
            }
        }
        else if (CaretMove.StartOfWord == caretMove)
        {
            // TODO
        }
        else if (CaretMove.EndOfWord == caretMove)
        {
            // TODO
        }
        else if (CaretMove.StartOfFile == caretMove)
        {
            if (Singleton.LineManager.GetLine(0, out Line startLine))
            {
                LineIndex = startLine.Index;
                CharIndex = 0;
            }
        }
        else if (CaretMove.EndOfFile == caretMove)
        {
            if (Singleton.LineManager.GetLine(Singleton.LineManager.Lines.Count - 1, out Line lastLine))
            {
                LineIndex = lastLine.Index;
                CharIndex = lastLine.Chars.Count();
            }
        }
        else if (CaretMove.PageUp == caretMove)
        {
            // TODO
        }
        else if (CaretMove.PageDown == caretMove)
        {
            // TODO
        }
        return this;
    }
}