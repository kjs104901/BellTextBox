using System.Numerics;
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

    public Caret SingleCaret(TextCoordinates textCoordinates = new())
    {
        if (Carets.Count > 1)
            Carets.RemoveRange(1, Carets.Count - 1);

        if (Carets.Count > 0)
        {
            Carets[0].Position = textCoordinates;
            Carets[0].Selection = textCoordinates;
        }
        else
        {
            Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
        }
        
        return Carets[0];
    }

    public void AddCaret(TextCoordinates textCoordinates)
    {
        Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
    }

    public void MoveCaretsPosition(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
    }
    
    public void MoveCaretsSelection(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
    }

    public void SelectRectangle(Vector2 startPosition, Vector2 endPosition)
    {
        Carets.Clear();
        // TODO select multiple lines
    }
}