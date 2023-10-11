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

    public static FontStyle DefaultFontStyle = new("Default") { R = 0.4f, G = 0.8f, B = 0.2f, A = 1.0f };
    public static FontStyle LineCommentFontStyle = new("LineComment") { R = 0.3f, G = 0.4f, B = 0.8f, A = 1.0f };
    public static FontStyle BlockCommentFontStyle = new("BlockComment") { R = 0.9f, G = 0.1f, B = 0.1f, A = 1.0f };

    public int CompareTo(FontStyle? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(_id, other._id, StringComparison.Ordinal);
    }
}