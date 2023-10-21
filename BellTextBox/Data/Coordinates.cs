namespace Bell.Data;

public struct PageCoordinates : IEquatable<PageCoordinates>
{
    public int Row;
    public int Column;

    public bool IsLineNumber;
    public bool IsFold;

    public bool Equals(PageCoordinates other) => Row == other.Row && Column == other.Column;
    public override bool Equals(object? obj) => obj is PageCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Row, Column);
    public static bool operator ==(PageCoordinates l, PageCoordinates r) => l.Row == r.Row && l.Column == r.Column;
    public static bool operator !=(PageCoordinates l, PageCoordinates r) => l.Row != r.Row || l.Column != r.Column; 
    public static bool operator <(PageCoordinates l, PageCoordinates r) => l.Row != r.Row ? l.Row < r.Row : l.Column < r.Column;
    public static bool operator >(PageCoordinates l, PageCoordinates r) => l.Row != r.Row ? l.Row > r.Row : l.Column > r.Column;
    public static bool operator <=(PageCoordinates l, PageCoordinates r) => l.Row != r.Row ? l.Row < r.Row : l.Column <= r.Column;
    public static bool operator >=(PageCoordinates l, PageCoordinates r) => l.Row != r.Row ? l.Row > r.Row : l.Column >= r.Column;
}

public struct TextCoordinates : IEquatable<TextCoordinates>
{
    public int LineIndex;
    public int CharIndex;

    public bool Equals(TextCoordinates other) => LineIndex == other.LineIndex && CharIndex == other.CharIndex;
    public override bool Equals(object? obj) => obj is TextCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(LineIndex, CharIndex);
    public static bool operator ==(TextCoordinates l, TextCoordinates r) => l.LineIndex == r.LineIndex && l.CharIndex == r.CharIndex;
    public static bool operator !=(TextCoordinates l, TextCoordinates r) => l.LineIndex != r.LineIndex || l.CharIndex != r.CharIndex; 
    public static bool operator <(TextCoordinates l, TextCoordinates r) => l.LineIndex != r.LineIndex ? l.LineIndex < r.LineIndex : l.CharIndex < r.CharIndex;
    public static bool operator >(TextCoordinates l, TextCoordinates r) => l.LineIndex != r.LineIndex ? l.LineIndex > r.LineIndex : l.CharIndex > r.CharIndex;
    public static bool operator <=(TextCoordinates l, TextCoordinates r) => l.LineIndex != r.LineIndex ? l.LineIndex < r.LineIndex : l.CharIndex <= r.CharIndex;
    public static bool operator >=(TextCoordinates l, TextCoordinates r) => l.LineIndex != r.LineIndex ? l.LineIndex > r.LineIndex : l.CharIndex >= r.CharIndex;
}