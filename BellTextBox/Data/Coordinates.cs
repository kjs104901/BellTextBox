namespace Bell.Data;

public struct TextCoordinates : IEquatable<TextCoordinates>
{
    public Line Line;
    public int CharIndex;

    public bool IsLineNumber;
    public bool IsFold;

    public bool Equals(TextCoordinates other) => Line.Index == other.Line.Index && CharIndex == other.CharIndex;
    public override bool Equals(object? obj) => obj is TextCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Line.Index, CharIndex);
    public static bool operator ==(TextCoordinates l, TextCoordinates r) => l.Line.Index == r.Line.Index && l.CharIndex == r.CharIndex;
    public static bool operator !=(TextCoordinates l, TextCoordinates r) => l.Line.Index != r.Line.Index || l.CharIndex != r.CharIndex; 
    public static bool operator <(TextCoordinates l, TextCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index < r.Line.Index : l.CharIndex < r.CharIndex;
    public static bool operator >(TextCoordinates l, TextCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index > r.Line.Index : l.CharIndex > r.CharIndex;
    public static bool operator <=(TextCoordinates l, TextCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index < r.Line.Index : l.CharIndex <= r.CharIndex;
    public static bool operator >=(TextCoordinates l, TextCoordinates r) => l.Line.Index != r.Line.Index ? l.Line.Index > r.Line.Index : l.CharIndex >= r.CharIndex;
}