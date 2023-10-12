using Bell.Coordinates;

namespace Bell.Carets;

public class CaretManager
{
    private readonly TextBox _textBox;

    private readonly List<Caret> _carets = new();

    public CaretManager(TextBox textBox)
    {
        _textBox = textBox;
    }

    public bool HasSelection() // TODO move this to each caret
    {
        return false;
    }

    public void SetCaret(TextCoordinates textCoordinates)
    {
        _carets.Clear();
        _carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
    }

    public bool GetCaretForDebug(out Caret caret)
    {
        if (_carets.Count > 0)
        {
            caret = _carets[0];
            return true;
        }
        caret = new Caret();
        return false;
    }
}