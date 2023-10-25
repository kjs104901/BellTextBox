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

    public Caret Clone()
    {
        return new Caret() { AnchorPosition = AnchorPosition, Position = Position };
    }

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
}