namespace Bell.Data;

public class Caret
{
    public PageCoordinates Selection;
    public PageCoordinates Position;

    public bool HasSelection => Selection != Position;

    public Caret Clone()
    {
        return new Caret() { Selection = Selection, Position = Position };
    }
}