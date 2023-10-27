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

        for (int i = 0; i < Singleton.CaretManager.Count; i++)
        {
            Caret caret = Singleton.CaretManager.GetCaret(i);
            caret.GetSortedPosition(out Coordinates start, out Coordinates end);

            if (caret.HasSelection)
            {
                if (start.IsSameAs(LineSub.Coordinates, Compare.ByLineSub))
                {
                    float startPosition = LineSub.GetCharPosition(start);
                    if (end.IsSameAs(LineSub.Coordinates, Compare.ByLineSub))
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (end.IsBiggerThan(LineSub.Coordinates, Compare.ByLineSub))
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
                else if (LineSub.Coordinates.IsBiggerThan(start, Compare.ByLineSub))
                {
                    if (end.IsSameAs(LineSub.Coordinates, Compare.ByLineSub))
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                    else if (end.IsBiggerThan(LineSub.Coordinates, Compare.ByLineSub))
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

            if (caret.AnchorPosition.IsSameAs(LineSub.Coordinates, Compare.ByLineSub))
            {
                float anchorPosition = LineSub.GetCharPosition(caret.AnchorPosition);
                lineSelection.HasCaretAnchor = true;
                lineSelection.CaretAnchorPosition = anchorPosition;
                if (caret.AnchorPosition.IsSameAs(end, Compare.ByLineSub) && fakeSelected &&
                    lineSelection.CaretAnchorPosition < 1.0f)
                    lineSelection.CaretAnchorPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
            }

            if (caret.Position.IsSameAs(LineSub.Coordinates, Compare.ByLineSub))
            {
                float caretPosition = LineSub.GetCharPosition(caret.Position);
                lineSelection.HasCaret = true;
                lineSelection.CaretPosition = caretPosition;
                if (caret.Position.IsSameAs(end, Compare.ByLineSub) && fakeSelected &&
                    lineSelection.CaretPosition < 1.0f)
                    lineSelection.CaretPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
            }
        }

        return lineSelection;
    }
}