using Bell.Data;

namespace Bell.Coordinates;

public struct ViewCoordinates
{
    public float X;
    public float Y;

    public ViewCoordinates(float x, float y)
    {
        X = x;
        Y = y;
    }

    public bool ToPageCoordinates(Page page, out PageCoordinates coordinates, out bool isLine, out bool isMarker)
    {
        //TODO
        //view.X + X - HeaderWidth;
        //view.Y + Y;
        coordinates = new PageCoordinates(0, 0);
        isLine = false;
        isMarker = false;
        return true;
    }
}