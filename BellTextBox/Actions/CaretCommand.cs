using Bell.Coordinates;

namespace Bell.Actions;

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
    
    Position,
    Selection
}

internal class MoveCaretSelectionCommand : Command
{
    private CaretMove _caretMove = CaretMove.None;
    private TextCoordinates _textCoordinates;
    
    public MoveCaretSelectionCommand(CaretMove caretMove)
    {
        _caretMove = caretMove;
    }
    
    public MoveCaretSelectionCommand(TextCoordinates textCoordinates)
    {
        _textCoordinates = textCoordinates;
    }
    
    public override void Do(TextBox textBox)
    {
    }
}

internal class MoveCaretPositionCommand : Command
{
    private CaretMove _caretMove = CaretMove.None;
    private TextCoordinates _textCoordinates;
    
    public MoveCaretPositionCommand(CaretMove caretMove)
    {
        _caretMove = caretMove;
    }
    
    public MoveCaretPositionCommand(TextCoordinates textCoordinates)
    {
        _textCoordinates = textCoordinates;
    }
    
    public override void Do(TextBox textBox)
    {
    }
}