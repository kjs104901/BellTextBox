namespace Bell.Data;

public class Caret
{
    public TextCoordinates Selection;
    public TextCoordinates Position;

    public bool HasSelection => Selection != Position;

    public Caret Clone()
    {
        return new Caret() { Selection = Selection, Position = Position };
    }
}