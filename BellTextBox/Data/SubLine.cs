using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class SubLine
{
    public int Row;
    
    public int WrapIndex;

    //public Line Line;
    //public int StartCharIndex;

    public LineCoordinates LineCoordinates;

    public float IndentWidth;

    public readonly List<TextBlockRender> TextBlockRenders = new();
    public readonly List<WhiteSpaceRender> WhiteSpaceRenders = new();

    public readonly List<char> Chars = new();
    public readonly List<float> CharWidths = new();

    public LineSelection LineSelection => LineSelectionCache.Get();
    public readonly Cache<LineSelection> LineSelectionCache;

    public SubLine(Line line, int charIndex, int wrapIndex, float indentWidth)
    {
        LineCoordinates = new LineCoordinates() { Line = line, CharIndex = charIndex };
        
        WrapIndex = wrapIndex;
        IndentWidth = indentWidth;

        LineSelectionCache = new Cache<LineSelection>(new(), UpdateLineSelection);
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
    
    private LineSelection UpdateLineSelection(LineSelection lineSelection)
    {
        lineSelection.Selected = false;
        lineSelection.SelectionStart = 0.0f;
        lineSelection.SelectionEnd = 0.0f;

        lineSelection.HasCaretAnchor = false;
        lineSelection.CaretAnchorPosition = 0.0f;
        lineSelection.HasCaret = false;
        lineSelection.CaretPosition = 0.0f;

        bool fakeSelected = false;
        
        foreach (Caret caret in ThreadLocal.CaretManager.Carets)
        {
            caret.GetSortedSelection(out var start, out var end);

            if (caret.HasSelection)
            {
                //float startPosition;
                //float endPosition;
                
                if (start.IsSameSubLine(LineCoordinates))
                {
                    float startPosition = GetCharPosition(start);
                    if (end.IsSameSubLine(LineCoordinates))
                    {
                        float endPosition = GetCharPosition(end);
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (LineCoordinates < end)
                    {
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = ThreadLocal.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }
                        lineSelection.Selected = true;
                    }
                }
                else if (start < LineCoordinates)
                {
                    if (LineCoordinates < end)
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = ThreadLocal.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }
                        lineSelection.Selected = true;
                    }
                    else if (end.IsSameSubLine(LineCoordinates))
                    {
                        float endPosition = GetCharPosition(end);
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                }
            }

            if (caret.AnchorPosition.IsSameSubLine(LineCoordinates))
            {
                float anchorPosition = GetCharPosition(caret.AnchorPosition);
                lineSelection.HasCaretAnchor = true;
                lineSelection.CaretAnchorPosition = anchorPosition;
                if (caret.AnchorPosition == end && fakeSelected && lineSelection.CaretAnchorPosition < 1.0f)
                    lineSelection.CaretAnchorPosition = ThreadLocal.FontManager.GetFontWhiteSpaceWidth();
            }

            if (caret.Position.IsSameSubLine(LineCoordinates))
            {
                float caretPosition = GetCharPosition(caret.Position);
                lineSelection.HasCaret = true;
                lineSelection.CaretPosition = caretPosition;
                if (caret.Position == end && fakeSelected && lineSelection.CaretPosition < 1.0f)
                    lineSelection.CaretPosition = ThreadLocal.FontManager.GetFontWhiteSpaceWidth();
            }
        }

        return lineSelection;
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
    public float Width;
}

public struct WhiteSpaceRender
{
    public char C;
    public float PosX;
}