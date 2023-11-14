using System.Numerics;

namespace Bell.Languages;

public readonly struct ColorStyle : IComparable<ColorStyle>
{
    private readonly float _r;
    private readonly float _g;
    private readonly float _b;
    private readonly float _a;

    public ColorStyle(float r, float g, float b, float a)
    {
        _r = r;
        _g = g;
        _b = b;
        _a = a;
    }

    public static ColorStyle None = new ColorStyle(0, 0, 0, 0);

    public int CompareTo(ColorStyle other)
    {
        var rComparison = _r.CompareTo(other._r);
        if (rComparison != 0) return rComparison;
        var gComparison = _g.CompareTo(other._g);
        if (gComparison != 0) return gComparison;
        var bComparison = _b.CompareTo(other._b);
        if (bComparison != 0) return bComparison;
        return _a.CompareTo(other._a);
    }
    
    public static bool operator ==(ColorStyle l, ColorStyle r) => l.CompareTo(r) == 0;
    public static bool operator !=(ColorStyle l, ColorStyle r) => l.CompareTo(r) != 0; 
    public override bool Equals(object? obj) => obj is ColorStyle other && (this == other);
    public override int GetHashCode() => HashCode.Combine(_r, _g, _b, _a);

    public readonly Vector4 ToVector() => new(_r, _g, _b, _a);
}