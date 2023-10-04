using Bell.Coordinates;

namespace Bell.Data;

public class Cursor
{
    public TextCoordinates Selection;
    public TextCoordinates Origin;

    public bool HasSelection => Selection != Origin;
}