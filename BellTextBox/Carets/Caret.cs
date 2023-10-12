using Bell.Coordinates;

namespace Bell.Carets;

public struct Caret
{
    public TextCoordinates Selection;
    public TextCoordinates Position;

    public bool HasSelection => Selection != Position;
}