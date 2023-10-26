using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class SubLine
{
    public LineCoordinates LineCoordinates;
    public readonly int WrapIndex;

    public readonly List<char> Chars = new();
    public readonly List<float> CharWidths = new();


    public SubLine(Line line, int charIndex, int wrapIndex)
    {
        LineCoordinates = new LineCoordinates() { Line = line, CharIndex = charIndex };
        
        WrapIndex = wrapIndex;
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

    public float GetCharPosition(LineCoordinates coordinates)
    {
        int index = coordinates.CharIndex - LineCoordinates.CharIndex;
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