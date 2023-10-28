namespace Bell.Data;

internal enum CaretMove
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

internal class Caret
{
    internal Coordinates AnchorPosition;
    internal Coordinates Position;

    internal bool HasSelection => !AnchorPosition.IsSameAs(Position);

    internal void RemoveSelection()
    {
        AnchorPosition = Position;
    }

    internal Caret Clone() => new() { AnchorPosition = AnchorPosition, Position = Position };
    
    internal static readonly Caret None = new Caret();
}