namespace Bell.Data;

public struct TextCoordinates : IEquatable<TextCoordinates>
{
    public int Row;
    public int Column;

    public int LineIndex;

    public bool IsLineNumber;
    public bool IsFold;

    public bool Equals(TextCoordinates other) => Row == other.Row && Column == other.Column;
    public override bool Equals(object? obj) => obj is TextCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Row, Column);
    public static bool operator ==(TextCoordinates l, TextCoordinates r) => l.Row == r.Row && l.Column == r.Column;
    public static bool operator !=(TextCoordinates l, TextCoordinates r) => l.Row != r.Row || l.Column != r.Column; 
    public static bool operator <(TextCoordinates l, TextCoordinates r) => l.Row != r.Row ? l.Row < r.Row : l.Column < r.Column;
    public static bool operator >(TextCoordinates l, TextCoordinates r) => l.Row != r.Row ? l.Row > r.Row : l.Column > r.Column;
    public static bool operator <=(TextCoordinates l, TextCoordinates r) => l.Row != r.Row ? l.Row < r.Row : l.Column <= r.Column;
    public static bool operator >=(TextCoordinates l, TextCoordinates r) => l.Row != r.Row ? l.Row > r.Row : l.Column >= r.Column;
}