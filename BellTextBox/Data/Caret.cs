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
    
    CharLeft,
    CharRight,
}

public class Caret
{
    public Coordinates AnchorPosition;
    public Coordinates Position;

    public bool HasSelection => !AnchorPosition.IsSameAs(Position);

    public void RemoveSelection()
    {
        AnchorPosition = Position;
    }

    public Caret Clone() => new() { AnchorPosition = AnchorPosition, Position = Position };
}