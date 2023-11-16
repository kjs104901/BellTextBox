using System.Globalization;
using System.Numerics;

namespace Bell.Themes;

public readonly struct ColorStyle : IComparable<ColorStyle>
{
    private readonly float _r;
    private readonly float _g;
    private readonly float _b;
    private readonly float _a;

    public ColorStyle(float r, float g, float b, float a = 1.0f)
    {
        _r = r;
        _g = g;
        _b = b;
        _a = a;
    }
    
    public ColorStyle(string hexColor)
    {
        if (hexColor.StartsWith("#"))
            hexColor = hexColor.Substring(1);

        _r = 0.0f;
        if (hexColor.Length >= 2)
            _r = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255.0f;
        
        _g = 0.0f;
        if (hexColor.Length >= 4)
            _g = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255.0f;
        
        _b = 0.0f;
        if (hexColor.Length >= 6)
            _b = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255.0f;
        
        _a = 1.0f;
        if (hexColor.Length >= 8)
            _a = int.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber) / 255.0f;
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