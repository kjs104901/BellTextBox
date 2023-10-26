namespace Bell.Data;

public enum CaretMove
{
    None,

    Up,
    Down,
    Left,
    Right,

    StartOfFile,
    EndOfFile,
    StartOfLine,
    EndOfLine,
    StartOfWord,
    EndOfWord,

    PageUp,
    PageDown,
}

public class Caret
{
    public LineCoordinates AnchorPosition;
    public LineCoordinates Position;

    public bool HasSelection => AnchorPosition != Position;

    public void GetSortedSelection(out LineCoordinates start, out LineCoordinates end)
    {
        if (AnchorPosition < Position)
        {
            start = AnchorPosition;
            end = Position;
        }
        else
        {
            start = Position;
            end = AnchorPosition;
        }
    }

    public void RemoveSelection()
    {
        AnchorPosition = Position;
    }
    
    public CaretCoordinates GetCaretCoordinates()
    {
        return new CaretCoordinates
        {
            PositionLineIndex = Position.Line.Index,
            PositionCharIndex = Position.CharIndex,
            AnchorPositionLineIndex = AnchorPosition.Line.Index,
            AnchorPositionCharIndex = AnchorPosition.CharIndex,
        };
    }
}

public struct CaretCoordinates
{
    public int PositionLineIndex;
    public int PositionCharIndex;
    public int AnchorPositionLineIndex;
    public int AnchorPositionCharIndex;
}