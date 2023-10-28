using Bell.Utils;

namespace Bell.Data;

internal class Row
{
    internal readonly List<TextBlockRender> TextBlockRenders = new();
    internal readonly List<WhiteSpaceRender> WhiteSpaceRenders = new();
    
    internal LineSelection LineSelection => LineSelectionCache.Get();
    internal readonly Cache<LineSelection> LineSelectionCache;

    internal LineSub LineSub;

    internal Row(LineSub lineSub)
    {
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

        for (int i = 0; i < CaretManager.Count; i++)
        {
            Caret caret = CaretManager.GetCaret(i);

            if (false == LineManager.GetLineSub(caret.AnchorPosition, out LineSub anchorLineSub) ||
                false == LineManager.GetLineSub(caret.Position, out LineSub lineSub))
            {
                Logger.Error("UpdateLineSelection: failed to get line");
                continue;
            }

            Coordinates start = caret.Position;
            Coordinates end = caret.AnchorPosition;
            LineSub startLineSub = lineSub;
            LineSub endLineSub = anchorLineSub;
            if (caret.Position.IsBiggerThan(caret.AnchorPosition)) // Swap start and end
            {
                start = caret.AnchorPosition;
                end = caret.Position;
                startLineSub = anchorLineSub;
                endLineSub = lineSub;
            }

            if (caret.HasSelection)
            {
                if (startLineSub == LineSub)
                {
                    float startPosition = LineSub.GetCharPosition(start);
                    if (endLineSub == LineSub)
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (endLineSub.IsBiggerThan(LineSub))
                    {
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = LineSub.CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }

                        lineSelection.Selected = true;
                    }
                }
                else if (LineSub.IsBiggerThan(startLineSub))
                {
                    if (endLineSub == LineSub)
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (endLineSub.IsBiggerThan(LineSub))
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = LineSub.CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                        {
                            lineSelection.SelectionEnd = FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }

                        lineSelection.Selected = true;
                    }
                }
            }

            if (anchorLineSub == LineSub)
            {
                float anchorPosition = LineSub.GetCharPosition(caret.AnchorPosition);
                lineSelection.HasCaretAnchor = true;
                lineSelection.CaretAnchorPosition = anchorPosition;
                if (endLineSub == anchorLineSub && fakeSelected && lineSelection.CaretAnchorPosition < 1.0f)
                    lineSelection.CaretAnchorPosition = FontManager.GetFontWhiteSpaceWidth();
            }
            
            if (lineSub == LineSub)
            {
                float caretPosition = LineSub.GetCharPosition(caret.Position);
                lineSelection.HasCaret = true;
                lineSelection.CaretPosition = caretPosition;
                if (endLineSub == lineSub && fakeSelected && lineSelection.CaretPosition < 1.0f)
                    lineSelection.CaretPosition = FontManager.GetFontWhiteSpaceWidth();
            }
        }
        return lineSelection;
    }
}