using Bell.Utils;

namespace Bell.Data;

public class Row
{
    public readonly List<TextBlockRender> TextBlockRenders = new();
    public readonly List<WhiteSpaceRender> WhiteSpaceRenders = new();

    public float IndentWidth = 0.0f;
    
    public LineSelection LineSelection => LineSelectionCache.Get();
    public readonly Cache<LineSelection> LineSelectionCache;

    public LineSub LineSub;
    
    public Row(float indentWidth, LineSub lineSub)
    {
        IndentWidth = indentWidth;
        LineSub = lineSub;
        
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
                
                if (start.IsSameLineSub(LineSub.LineCoordinates))
                {
                    float startPosition = LineSub.GetCharPosition(start);
                    if (end.IsSameLineSub(LineSub.LineCoordinates))
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (LineSub.LineCoordinates < end)
                    {
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = LineSub.CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = Singleton.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }
                        lineSelection.Selected = true;
                    }
                }
                else if (start < LineSub.LineCoordinates)
                {
                    if (end.IsSameLineSub(LineSub.LineCoordinates))
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (LineSub.LineCoordinates < end)
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = LineSub.CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = Singleton.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }
                        lineSelection.Selected = true;
                    }
                }
            }

            if (caret.AnchorPosition.IsSameLineSub(LineSub.LineCoordinates))
            {
                float anchorPosition = LineSub.GetCharPosition(caret.AnchorPosition);
                lineSelection.HasCaretAnchor = true;
                lineSelection.CaretAnchorPosition = anchorPosition;
                if (caret.AnchorPosition == end && fakeSelected && lineSelection.CaretAnchorPosition < 1.0f)
                    lineSelection.CaretAnchorPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
            }

            if (caret.Position.IsSameLineSub(LineSub.LineCoordinates))
            {
                float caretPosition = LineSub.GetCharPosition(caret.Position);
                lineSelection.HasCaret = true;
                lineSelection.CaretPosition = caretPosition;
                if (caret.Position == end && fakeSelected && lineSelection.CaretPosition < 1.0f)
                    lineSelection.CaretPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
            }
        }

        return lineSelection;
    }
}