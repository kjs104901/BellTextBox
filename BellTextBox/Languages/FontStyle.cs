namespace Bell.Languages;

public class FontStyle : IComparable<FontStyle>
{
    private readonly string _id;
    
    public float R;
    public float G;
    public float B;
    public float A;

    public bool Bold;
    public bool Italic;

    public FontStyle(string id)
    {
        _id = id;
    }

    public static FontStyle DefaultFontStyle = new FontStyle("Default");
    public static FontStyle LineCommentFontStyle = new FontStyle("LineComment");
    public static FontStyle BlockCommentFontStyle = new FontStyle("BlockComment");

    public int CompareTo(FontStyle? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(_id, other._id, StringComparison.Ordinal);
    }
}