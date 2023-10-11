using Bell.Data;

namespace Bell.Coordinates;

public struct PageCoordinates
{
    public float X;
    public float Y;

    public PageCoordinates(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public bool ToTextCoordinates(Text? text, out TextCoordinates coordinates)
    {
        //TODO
        // calculate line and column
        coordinates = new TextCoordinates(0, 0);
        return true;
    }

    public bool ToLineNumber(Text text, out int lineNumber)
    {
        // TODO find line number
        lineNumber = 0;
        return true;
    }
}