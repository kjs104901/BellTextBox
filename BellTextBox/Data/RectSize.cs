namespace Bell.Data;

public struct RectSize : IEqualityComparer<RectSize>
{
    public float Width;
    public float Height;

    public RectSize(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public bool Equals(RectSize x, RectSize y)
    {
        return x.Width.Equals(y.Width) && x.Height.Equals(y.Height);
    }

    public int GetHashCode(RectSize obj)
    {
        return HashCode.Combine(obj.Width, obj.Height);
    }
}