namespace Bell.Data;

public class Caret
{
    public TextCoordinates AnchorPosition;
    public TextCoordinates Position;

    public bool HasSelection => AnchorPosition != Position;

    public Caret Clone()
    {
        return new Caret() { AnchorPosition = AnchorPosition, Position = Position };
    }

    public void GetSortedSelection(out TextCoordinates start, out TextCoordinates end)
    {
        if (AnchorPosition < Position)
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
}