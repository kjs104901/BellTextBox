using Bell.Coordinates;

namespace Bell.Data;

public class Caret
{
    public TextCoordinates Selection;
    public TextCoordinates Position;

    public bool HasSelection => Selection != Position;
}