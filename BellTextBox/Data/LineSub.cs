using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

internal class LineSub
{
    internal Coordinates Coordinates;

    internal readonly List<char> Chars = new();
    internal readonly List<float> CharWidths = new();
    
    internal readonly float IndentWidth;
    
    internal static readonly LineSub None = new(-1, -1, -1, 0.0f);
    
    internal LineSub(int lineIndex, int charIndex, int lineSubIndex, float indentWidth)
    {
        Coordinates = new Coordinates(lineIndex, charIndex, lineSubIndex);
        IndentWidth = indentWidth;
    }
    
    internal int GetCharIndex(float position)
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

    internal float GetCharPosition(Coordinates coordinates)
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

    internal bool IsBiggerThan(LineSub other)
    {
        if (Coordinates.LineIndex != other.Coordinates.LineIndex)
            return Coordinates.LineIndex > other.Coordinates.LineIndex;
        return Coordinates.LineSubIndex > other.Coordinates.LineSubIndex;
    }
}

internal struct LineSelection
{
    internal bool Selected;
    internal float SelectionStart;
    internal float SelectionEnd;

    internal bool HasCaretAnchor;
    internal float CaretAnchorPosition;
    internal bool HasCaret;
    internal float CaretPosition;
}

internal struct TextBlockRender
{
    internal string Text;
    internal ColorStyle ColorStyle;

    internal float PosX;
}

internal struct WhiteSpaceRender
{
    internal char C;
    internal float PosX;
}