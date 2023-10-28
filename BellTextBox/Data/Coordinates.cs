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
        if (false == Singleton.LineManager.GetLineSub(this, out LineSub lineSub) ||
            false == Singleton.LineManager.GetLineSub(other, out LineSub otherLineSub))
        {
            Logger.Error($"IsSameAs: failed to get line sub: {LineIndex},{CharIndex} {other.LineIndex},{other.CharIndex}");
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
        if (false == Singleton.LineManager.GetLineSub(this, out LineSub lineSub) ||
            false == Singleton.LineManager.GetLineSub(other, out LineSub otherLineSub))
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

        if (CharIndex < 0 || CharIndex > line.Chars.Count + 1)
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

        Logger.Info("FindMove: " + caretMove + " " + count + " " + newCoordinates.LineIndex + " " +
                    newCoordinates.CharIndex);

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
            if (false == Singleton.LineManager.GetLine(LineIndex, out Line line))
                return this;

            // End of line
            if (line.Chars.Count <= CharIndex)
            {
                // End of file
                if (false == Singleton.LineManager.GetLine(LineIndex + 1, out Line nextLine))
                    return this;

                LineIndex = nextLine.Index;
                CharIndex = 0;
                LineSubIndex = -1;
                return this;
            }

            // Has next line sub
            if (Singleton.LineManager.GetLineSub(this, out LineSub lineSub) &&
                CharIndex == lineSub.Coordinates.CharIndex + lineSub.Chars.Count &&
                Singleton.LineManager.GetLineSub(LineIndex, lineSub.Coordinates.LineSubIndex + 1,
                    out LineSub nextLineSub))
            {
                LineSubIndex = nextLineSub.Coordinates.LineSubIndex;
                return this;
            }

            CharIndex++;
            return this;
        }

        if (CaretMove.Left == caretMove)
        {
            // Start of line
            if (CharIndex <= 0)
            {
                // Start of file
                if (false == Singleton.LineManager.GetLine(LineIndex - 1, out Line prevLine))
                    return this;

                LineIndex = prevLine.Index;
                CharIndex = prevLine.Chars.Count;
                LineSubIndex = -1;
                return this;
            }

            // Has prev line sub
            if (Singleton.LineManager.GetLineSub(this, out LineSub lineSub) &&
                CharIndex == lineSub.Coordinates.CharIndex &&
                Singleton.LineManager.GetLineSub(LineIndex, lineSub.Coordinates.LineSubIndex - 1,
                    out LineSub prevLineSub))
            {
                LineSubIndex = prevLineSub.Coordinates.LineSubIndex;
                return this;
            }

            CharIndex--;
            return this;
        }

        if (CaretMove.CharRight == caretMove)
        {
            if (false == Singleton.LineManager.GetLine(LineIndex, out Line line))
                return this;
            
            // End of line
            if (line.Chars.Count <= CharIndex)
            {
                // End of file
                if (false == Singleton.LineManager.GetLine(LineIndex + 1, out Line nextLine))
                    return this;
                
                LineIndex = nextLine.Index;
                CharIndex = 0;
                LineSubIndex = -1;
            }
            
            CharIndex++;
            LineSubIndex = -1;
            return this;
        }
        
        if (CaretMove.CharLeft == caretMove)
        {
            // Start of line
            if (CharIndex <= 0)
            {
                // Start of file
                if (false == Singleton.LineManager.GetLine(LineIndex - 1, out Line prevLine))
                    return this;
                
                LineIndex = prevLine.Index;
                CharIndex = prevLine.Chars.Count;
                LineSubIndex = -1;
            }
            CharIndex--;
            LineSubIndex = -1;
            return this;
        }

        if (CaretMove.Up == caretMove)
        {
            // Has prev line sub
            if (Singleton.LineManager.GetLineSub(this, out LineSub lineSub) &&
                lineSub.Coordinates.LineSubIndex > 0 &&
                Singleton.LineManager.GetLineSub(LineIndex, lineSub.Coordinates.LineSubIndex - 1,
                    out LineSub prevLineSub))
            {
                float indentDiff = lineSub.IndentWidth - prevLineSub.IndentWidth;
                int subCharIndex = prevLineSub.GetCharIndex(lineSub.GetCharPosition(this) + indentDiff);
                CharIndex = prevLineSub.Coordinates.CharIndex + subCharIndex;
                LineSubIndex = lineSub.Coordinates.LineSubIndex - 1;
                return this;
            }

            // Start of file
            if (false == Singleton.LineManager.GetLine(LineIndex - 1, out Line prevLine))
                return this;

            // Last line sub of prev line
            if (Singleton.LineManager.GetLineSub(prevLine.Index, prevLine.LineSubs.Count - 1,
                    out LineSub prevLineLastLineSub))
            {
                float indentDiff = lineSub.IndentWidth - prevLineLastLineSub.IndentWidth;
                int subCharIndex = prevLineLastLineSub.GetCharIndex(lineSub.GetCharPosition(this) + indentDiff);
                LineIndex = prevLine.Index;
                CharIndex = prevLineLastLineSub.Coordinates.CharIndex + subCharIndex;
                LineSubIndex = prevLineLastLineSub.Coordinates.LineSubIndex;
                return this;
            }
            Logger.Error("Failed to find prev line sub");
            return this;
        }

        if (CaretMove.Down == caretMove)
        {
            // Has next line sub
            if (Singleton.LineManager.GetLineSub(this, out LineSub lineSub) &&
                Singleton.LineManager.GetLine(LineIndex, out Line line) &&
                lineSub.Coordinates.LineSubIndex + 1 < line.LineSubs.Count &&
                Singleton.LineManager.GetLineSub(LineIndex, lineSub.Coordinates.LineSubIndex + 1,
                    out LineSub nextLineSub))
            {
                float indentDiff = lineSub.IndentWidth - nextLineSub.IndentWidth;
                int subCharIndex = nextLineSub.GetCharIndex(lineSub.GetCharPosition(this) + indentDiff);
                CharIndex = nextLineSub.Coordinates.CharIndex + subCharIndex;
                LineSubIndex = lineSub.Coordinates.LineSubIndex + 1;
                return this;
            }

            // End of file
            if (false == Singleton.LineManager.GetLine(LineIndex + 1, out Line nextLine))
                return this;

            // First line sub of next line
            if (Singleton.LineManager.GetLineSub(nextLine.Index, 0, out LineSub nextLineFirstLineSub))
            {
                float indentDiff = lineSub.IndentWidth - nextLineFirstLineSub.IndentWidth;
                int subCharIndex = nextLineFirstLineSub.GetCharIndex(lineSub.GetCharPosition(this) + indentDiff);
                LineIndex = nextLine.Index;
                CharIndex = nextLineFirstLineSub.Coordinates.CharIndex + subCharIndex;
                LineSubIndex = nextLineFirstLineSub.Coordinates.LineSubIndex;
                return this;
            }

            Logger.Error("Failed to find next line sub");
            return this;
        }

        if (CaretMove.StartOfLine == caretMove)
        {
            CharIndex = 0;
            LineSubIndex = -1;
            return this;
        }

        if (CaretMove.EndOfLine == caretMove)
        {
            if (false == Singleton.LineManager.GetLine(LineIndex, out Line line))
                return this;

            CharIndex = line.Chars.Count;
            LineSubIndex = -1;
            return this;
        }

        if (CaretMove.StartOfWord == caretMove)
        {
            // TODO
            return this;
        }

        if (CaretMove.EndOfWord == caretMove)
        {
            // TODO
            return this;
        }

        if (CaretMove.StartOfFile == caretMove)
        {
            if (false == Singleton.LineManager.GetLine(0, out Line startLine))
                return this;

            LineIndex = startLine.Index;
            CharIndex = 0;
            return this;
        }

        if (CaretMove.EndOfFile == caretMove)
        {
            if (false == Singleton.LineManager.GetLine(Singleton.LineManager.Lines.Count - 1, out Line lastLine))
                return this;

            LineIndex = lastLine.Index;
            CharIndex = lastLine.Chars.Count();
            return this;
        }

        if (CaretMove.PageUp == caretMove)
        {
            // TODO
            return this;
        }

        if (CaretMove.PageDown == caretMove)
        {
            // TODO
            return this;
        }

        return this;
    }
}