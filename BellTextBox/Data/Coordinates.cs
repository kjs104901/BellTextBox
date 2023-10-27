using Bell.Utils;

namespace Bell.Data;

public enum Compare
{
    ByLine,
    ByLineSub,
    ByChar,
}

public struct Coordinates
{
    public int LineIndex;
    public int CharIndex;
    
    public bool IsSameAs(Coordinates other, Compare compare)
    {
        if (Compare.ByLine == compare)
        {
            return LineIndex == other.LineIndex;
        }
        
        if (false == Singleton.LineManager.GetLine(LineIndex, out Line line) ||
            false == line.GetLineSub(CharIndex, out LineSub lineSub) ||
            false == Singleton.LineManager.GetLine(other.LineIndex, out Line otherLine) ||
            false == otherLine.GetLineSub(other.CharIndex, out LineSub otherLineSub))
        {
            Logger.Error($"IsSameAs: failed to get line: {LineIndex} or {other.LineIndex}");
            return false;
        }
        
        if (Compare.ByLineSub == compare)
        {
            if (LineIndex != other.LineIndex)
                return false;
            
            return lineSub.LineSubIndex == otherLineSub.LineSubIndex;
        }

        if (Compare.ByChar == compare)
        {
            if (LineIndex != other.LineIndex)
                return false;
            
            if (lineSub.LineSubIndex != otherLineSub.LineSubIndex)
                return false;

            return CharIndex == other.CharIndex;
        }
        return true;
    }

    public bool IsBiggerThan(Coordinates other, Compare compare)
    {
        if (Compare.ByLine == compare)
        {
            return LineIndex > other.LineIndex;
        }
        
        if (false == Singleton.LineManager.GetLine(LineIndex, out Line line) ||
            false == line.GetLineSub(CharIndex, out LineSub lineSub) ||
            false == Singleton.LineManager.GetLine(other.LineIndex, out Line otherLine) ||
            false == otherLine.GetLineSub(other.CharIndex, out LineSub otherLineSub))
        {
            Logger.Error($"IsBiggerThan: failed to get line: {LineIndex} or {other.LineIndex}");
            return false;
        }
        
        if (Compare.ByLineSub == compare)
        {
            if (LineIndex != other.LineIndex)
                return LineIndex > other.LineIndex;
            
            return lineSub.LineSubIndex > otherLineSub.LineSubIndex;
        }
        
        if (Compare.ByChar == compare)
        {
            if (LineIndex != other.LineIndex)
                return LineIndex > other.LineIndex;

            if (lineSub.LineSubIndex != otherLineSub.LineSubIndex)
                return lineSub.LineSubIndex > otherLineSub.LineSubIndex;
            
            return CharIndex > other.CharIndex;
        }
        
        return true;
    }
    
    public Coordinates FindMove(CaretMove caretMove, int count = 1)
    {
        Coordinates newCoordinates = this;
        for (int i = 0; i < count; i++)
        {
            newCoordinates = newCoordinates.FindMoveSingle(caretMove);
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