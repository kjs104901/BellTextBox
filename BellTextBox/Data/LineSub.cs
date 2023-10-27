using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class LineSub
{
    // TODO Update LineIndex when Line.Index changes
    public Coordinates Coordinates;
    public readonly int LineSubIndex;

    public readonly List<char> Chars = new();
    public readonly List<float> CharWidths = new();
    
    public static readonly LineSub None = new(-1, -1, -1);
    
    public LineSub(int lineIndex, int charIndex, int lineSubIndex)
    {
        Coordinates = new Coordinates() { LineIndex = lineIndex, CharIndex = charIndex };
        LineSubIndex = lineSubIndex;
    }
    
    public int GetCharIndex(float position)
    {
        float current = 0.0f;
        for (var i = 0; i < CharWidths.Count; i++)
        {
            current += CharWidths[i];
            if (current > position)
                return i;
        }
        return CharWidths.Count;
    }

    public float GetCharPosition(Coordinates coordinates)
    {
        int index = coordinates.CharIndex - Coordinates.CharIndex;
        float position = 0.0f;
        for (var i = 0; i < index; i++)
        {
            position += CharWidths[i];
        }
        return position;
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