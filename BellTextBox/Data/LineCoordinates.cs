using Bell.Utils;

namespace Bell.Data;

public struct LineCoordinates : IEquatable<LineCoordinates>
{
    public Line Line;
    public int CharIndex;
    
    public bool Equals(LineCoordinates other) => Line.Index == other.Line.Index && CharIndex == other.CharIndex;
    public override bool Equals(object? obj) => obj is LineCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Line.Index, CharIndex);
    public static bool operator ==(LineCoordinates l, LineCoordinates r) => l.Line.Index == r.Line.Index && l.CharIndex == r.CharIndex;
    public static bool operator !=(LineCoordinates l, LineCoordinates r) => l.Line.Index != r.Line.Index || l.CharIndex != r.CharIndex; 
    public static bool operator <(LineCoordinates l, LineCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index < r.Line.Index : l.CharIndex < r.CharIndex;
    public static bool operator >(LineCoordinates l, LineCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index > r.Line.Index : l.CharIndex > r.CharIndex;
    public static bool operator <=(LineCoordinates l, LineCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index < r.Line.Index : l.CharIndex <= r.CharIndex;
    public static bool operator >=(LineCoordinates l, LineCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index > r.Line.Index : l.CharIndex >= r.CharIndex;
    
    public SubLine GetSubLine()
    {
        var foundSubLine = Line.SubLines[0];
        foreach (var subLine in Line.SubLines)
        {
            if (subLine.LineCoordinates.CharIndex > CharIndex)
                break;
            foundSubLine = subLine;
        }
        return foundSubLine;
    }

    public bool IsSameSubLine(LineCoordinates other)
    {
        return GetSubLine() == other.GetSubLine();
    }
    
    // TODO change to Move?, change CaretMove name?
    public LineCoordinates FindCoordinates(CaretMove caretMove)
    {
        LineCoordinates newCoordinates = new LineCoordinates() { Line = Line, CharIndex = CharIndex };
        
        if (CaretMove.Right == caretMove)
        {
            // Check end of line
            if (newCoordinates.CharIndex < newCoordinates.Line.Chars.Count)
            {
                newCoordinates.CharIndex++;
            }
            else
            {
                // Check end of file
                if (ThreadLocal.LineManager.GetLine(newCoordinates.Line.Index + 1, out Line nextLine))
                {
                    newCoordinates.Line = nextLine;
                    newCoordinates.CharIndex = 0;
                }
            }
        }
        else if (CaretMove.Left == caretMove)
        {
            // Check start of line
            if (newCoordinates.CharIndex > 0)
            {
                newCoordinates.CharIndex--;
            }
            else
            {
                // Check start of file
                if (ThreadLocal.LineManager.GetLine(newCoordinates.Line.Index - 1, out Line prevLine))
                {
                    newCoordinates.Line = prevLine;
                    newCoordinates.CharIndex = prevLine.Chars.Count;
                }
            }
        }
        else if (CaretMove.Up == caretMove)
        {
            // Check start of file
            if (ThreadLocal.LineManager.GetLine(newCoordinates.Line.Index - 1, out Line prevLine))
            {
                newCoordinates.Line = prevLine;
                newCoordinates.CharIndex = Math.Min(newCoordinates.CharIndex, prevLine.Chars.Count);
            }
        }
        else if (CaretMove.Down == caretMove)
        {
            // Check end of file
            if (ThreadLocal.LineManager.GetLine(newCoordinates.Line.Index + 1, out Line nextLine))
            {
                newCoordinates.Line = nextLine;
                newCoordinates.CharIndex = Math.Min(newCoordinates.CharIndex, nextLine.Chars.Count);
            }
        }
        else if (CaretMove.StartOfLine == caretMove)
        {
            newCoordinates.CharIndex = 0;
        }
        else if (CaretMove.EndOfLine == caretMove)
        {
            newCoordinates.CharIndex = newCoordinates.Line.Chars.Count;
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
            if (ThreadLocal.LineManager.GetLine(0, out Line startLine))
            {
                newCoordinates.Line = startLine;
                newCoordinates.CharIndex = 0;
            }
        }
        else if (CaretMove.EndOfFile == caretMove)
        {
            if (ThreadLocal.LineManager.GetLine(ThreadLocal.LineManager.Lines.Count - 1, out Line lastLine))
            {
                newCoordinates.Line = lastLine;
                newCoordinates.CharIndex = lastLine.Chars.Count();
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
        
        return newCoordinates;
    }
}