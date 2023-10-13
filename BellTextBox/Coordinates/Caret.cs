using Bell.Coordinates;

namespace Bell.Carets;

public class Caret
{
    public TextCoordinates Selection;
    public TextCoordinates Position;

    public bool HasSelection => Selection != Position;
}