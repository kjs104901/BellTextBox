using Bell.Utils;

namespace Bell.Data;

public class Row
{
    public readonly List<TextBlockRender> TextBlockRenders = new();
    public readonly List<WhiteSpaceRender> WhiteSpaceRenders = new();

    public float IndentWidth = 0.0f;
    
    public LineSelection LineSelection => LineSelectionCache.Get();
    public readonly Cache<LineSelection> LineSelectionCache;

    public SubLine SubLine;
    
    public Row(float indentWidth, SubLine subLine)
    {
        IndentWidth = indentWidth;
        SubLine = subLine;
        
        LineSelectionCache = new(new(), UpdateLineSelection);
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
        
        foreach (Caret caret in Singleton.CaretManager.Carets)
        {
            caret.GetSortedSelection(out var start, out var end);

            if (caret.HasSelection)
            {
                //float startPosition;
                //float endPosition;
                
                if (start.IsSameSubLine(SubLine.LineCoordinates))
                {
                    float startPosition = SubLine.GetCharPosition(start);
                    if (end.IsSameSubLine(SubLine.LineCoordinates))
                    {
                        float endPosition = SubLine.GetCharPosition(end);
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (SubLine.LineCoordinates < end)
                    {
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = SubLine.CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = Singleton.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }
                        lineSelection.Selected = true;
                    }
                }
                else if (start < SubLine.LineCoordinates)
                {
                    if (end.IsSameSubLine(SubLine.LineCoordinates))
                    {
                        float endPosition = SubLine.GetCharPosition(end);
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (SubLine.LineCoordinates < end)
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = SubLine.CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = Singleton.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }
                        lineSelection.Selected = true;
                    }
                }
            }

            if (caret.AnchorPosition.IsSameSubLine(SubLine.LineCoordinates))
            {
                float anchorPosition = SubLine.GetCharPosition(caret.AnchorPosition);
                lineSelection.HasCaretAnchor = true;
                lineSelection.CaretAnchorPosition = anchorPosition;
                if (caret.AnchorPosition == end && fakeSelected && lineSelection.CaretAnchorPosition < 1.0f)
                    lineSelection.CaretAnchorPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
            }

            if (caret.Position.IsSameSubLine(SubLine.LineCoordinates))
            {
                float caretPosition = SubLine.GetCharPosition(caret.Position);
                lineSelection.HasCaret = true;
                lineSelection.CaretPosition = caretPosition;
                if (caret.Position == end && fakeSelected && lineSelection.CaretPosition < 1.0f)
                    lineSelection.CaretPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
            }
        }

        return lineSelection;
    }
}