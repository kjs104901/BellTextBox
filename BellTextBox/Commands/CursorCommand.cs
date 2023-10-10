using Bell.Coordinates;

namespace Bell.Commands;

internal enum CursorMove
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
    
    Origin,
    Selection
}

internal class MoveCursorSelectionCommand : Command
{
    private CursorMove _cursorMove = CursorMove.None;
    private TextCoordinates _textCoordinates;
    
    public MoveCursorSelectionCommand(CursorMove cursorMove)
    {
        _cursorMove = cursorMove;
    }
    
    public MoveCursorSelectionCommand(TextCoordinates textCoordinates)
    {
        _textCoordinates = textCoordinates;
    }
    
    public override void Do(TextBox textBox)
    {
    }
}

internal class MoveCursorOriginCommand : Command
{
    private CursorMove _cursorMove = CursorMove.None;
    private TextCoordinates _textCoordinates;
    
    public MoveCursorOriginCommand(CursorMove cursorMove)
    {
        _cursorMove = cursorMove;
    }
    
    public MoveCursorOriginCommand(TextCoordinates textCoordinates)
    {
        _textCoordinates = textCoordinates;
    }
    
    public override void Do(TextBox textBox)
    {
    }
}