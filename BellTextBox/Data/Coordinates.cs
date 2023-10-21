namespace Bell.Data;

public struct PageCoordinates : IEquatable<PageCoordinates>
{
    public int RowIndex;
    public int ColumnIndex;

    public bool Equals(PageCoordinates other) => RowIndex == other.RowIndex && ColumnIndex == other.ColumnIndex;
    public override bool Equals(object? obj) => obj is PageCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(RowIndex, ColumnIndex);
    public static bool operator ==(PageCoordinates l, PageCoordinates r) => l.RowIndex == r.RowIndex && l.ColumnIndex == r.ColumnIndex;
    public static bool operator !=(PageCoordinates l, PageCoordinates r) => l.RowIndex != r.RowIndex || l.ColumnIndex != r.ColumnIndex; 
    public static bool operator <(PageCoordinates l, PageCoordinates r) => l.RowIndex != r.RowIndex ? l.RowIndex < r.RowIndex : l.ColumnIndex < r.ColumnIndex;
    public static bool operator >(PageCoordinates l, PageCoordinates r) => l.RowIndex != r.RowIndex ? l.RowIndex > r.RowIndex : l.ColumnIndex > r.ColumnIndex;
    public static bool operator <=(PageCoordinates l, PageCoordinates r) => l.RowIndex != r.RowIndex ? l.RowIndex < r.RowIndex : l.ColumnIndex <= r.ColumnIndex;
    public static bool operator >=(PageCoordinates l, PageCoordinates r) => l.RowIndex != r.RowIndex ? l.RowIndex > r.RowIndex : l.ColumnIndex >= r.ColumnIndex;
}

public struct TextCoordinates : IEquatable<TextCoordinates>
{
    public int LineIndex;
    public int CharIndex;

    public bool IsLineNumber;
    public bool IsFold;

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