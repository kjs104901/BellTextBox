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
    public Coordinates AnchorPosition;
    public Coordinates Position;

    public bool HasSelection => !AnchorPosition.IsSameAs(Position, Compare.ByChar);

    public void GetSortedPosition(out Coordinates start, out Coordinates end)
    {
        if (Position.IsBiggerThan(AnchorPosition, Compare.ByChar))
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

    public Caret Clone() => new() { AnchorPosition = AnchorPosition, Position = Position };
}