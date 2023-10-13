using Bell.Coordinates;

namespace Bell.Carets;

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
    
    Position,
    Selection
}

public class CaretManager
{
    private readonly TextBox _textBox;

    public readonly List<Caret> Carets = new();

    public CaretManager(TextBox textBox)
    {
        _textBox = textBox;
    }

    public void SetCaret(TextCoordinates textCoordinates)
    {
        Carets.Clear();
        Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
    }

    public void AddCaret(TextCoordinates textCoordinates)
    {
        Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
    }

    public void MoveCarets(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
    }
}