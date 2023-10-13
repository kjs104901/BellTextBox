using System.Numerics;

namespace Bell.Render;

public struct ColorStyle : IComparable<ColorStyle>
{
    public float R;
    public float G;
    public float B;
    public float A;

    public ColorStyle(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public int CompareTo(ColorStyle other)
    {
        var rComparison = R.CompareTo(other.R);
        if (rComparison != 0) return rComparison;
        var gComparison = G.CompareTo(other.G);
        if (gComparison != 0) return gComparison;
        var bComparison = B.CompareTo(other.B);
        if (bComparison != 0) return bComparison;
        return A.CompareTo(other.A);
    }
    
    public static bool operator ==(ColorStyle l, ColorStyle r) => l.CompareTo(r) == 0;
    public static bool operator !=(ColorStyle l, ColorStyle r) => l.CompareTo(r) != 0; 

    public readonly Vector4 ToVector() => new(R, G, B, A);
}