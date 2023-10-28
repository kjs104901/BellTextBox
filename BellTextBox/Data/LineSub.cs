using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class LineSub
{
    public Coordinates Coordinates;

    public readonly List<char> Chars = new();
    public readonly List<float> CharWidths = new();
    
    public static readonly LineSub None = new(-1, -1, -1);
    
    public LineSub(int lineIndex, int charIndex, int lineSubIndex)
    {
        Coordinates = new Coordinates(lineIndex, charIndex, lineSubIndex);
    }
    
    public int GetCharIndex(float position)
    {
        float current = 0.0f;
        for (var i = 0; i < CharWidths.Count; i++)
        {
            float charWidth = CharWidths[i];
            current += charWidth;
            if (current > position + charWidth * 0.5f)
                return i;
        }
        return CharWidths.Count;
    }

    public float GetCharPosition(Coordinates coordinates)
    {
        int index = coordinates.CharIndex - Coordinates.CharIndex;
        if (index < 0)
            return 0.0f;
        
        float position = 0.0f;
        for (var i = 0; i < CharWidths.Count; i++)
        {
            if (i == index)
                break;
            position += CharWidths[i];
        }
        return position;
    }

    public bool IsBiggerThan(LineSub other)
    {
        if (Coordinates.LineIndex != other.Coordinates.LineIndex)
            return Coordinates.LineIndex > other.Coordinates.LineIndex;
        return Coordinates.LineSubIndex > other.Coordinates.LineSubIndex;
    }
}

public struct LineSelection
{
    public bool Selected;
    public float SelectionStart;
    public float SelectionEnd;

    public bool HasCaretAnchor;
    public float CaretAnchorPosition;
    public bool HasCaret;
    public float CaretPosition;
}

public struct TextBlockRender
{
    public string Text;
    public ColorStyle ColorStyle;

    public float PosX;
}

public struct WhiteSpaceRender
{
    public char C;
    public float PosX;
}