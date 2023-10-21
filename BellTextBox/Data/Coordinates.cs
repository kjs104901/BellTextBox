namespace Bell.Data;

public struct PageCoordinates : IEquatable<PageCoordinates>
{
    public int Row;
    public int Column;

    public int LineIndex;

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