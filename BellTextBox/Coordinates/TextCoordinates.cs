namespace Bell.Coordinates;

public struct TextCoordinates : IEquatable<TextCoordinates>
{
    public uint Row;
    public uint Column;

    public TextCoordinates(uint row, uint column)
    {
        Row = row;
        Column = column;
    }

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