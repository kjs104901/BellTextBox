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

            if (false == Singleton.LineManager.GetSubLine(caret.AnchorPosition, out LineSub anchorLineSub) ||
                false == Singleton.LineManager.GetSubLine(caret.Position, out LineSub lineSub))
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
                            lineSelection.SelectionEnd = Singleton.FontManager.GetFontWhiteSpaceWidth();
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
                            lineSelection.SelectionEnd = Singleton.FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }

                        lineSelection.Selected = true;
                    }
                }
            }

            if (caret.AnchorPosition.LineSubIndex >= 0)
            {
                if (caret.AnchorPosition.LineIndex == LineSub.Coordinates.LineIndex &&
                    caret.AnchorPosition.LineSubIndex == LineSub.Coordinates.LineSubIndex)
                {
                    float anchorPosition = LineSub.GetCharPosition(caret.AnchorPosition);
                    lineSelection.HasCaretAnchor = true;
                    lineSelection.CaretAnchorPosition = anchorPosition;
                    if (endLineSub == anchorLineSub && fakeSelected && lineSelection.CaretAnchorPosition < 1.0f)
                        lineSelection.CaretAnchorPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
                }
            }
            else
            {
                if (anchorLineSub == LineSub)
                {
                    float anchorPosition = LineSub.GetCharPosition(caret.AnchorPosition);
                    lineSelection.HasCaretAnchor = true;
                    lineSelection.CaretAnchorPosition = anchorPosition;
                    if (endLineSub == anchorLineSub && fakeSelected && lineSelection.CaretAnchorPosition < 1.0f)
                        lineSelection.CaretAnchorPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
                }
            }

            if (caret.Position.LineSubIndex >= 0)
            {
                if (caret.Position.LineIndex == LineSub.Coordinates.LineIndex && 
                    caret.Position.LineSubIndex == LineSub.Coordinates.LineSubIndex)
                {
                    float caretPosition = LineSub.GetCharPosition(caret.Position);
                    lineSelection.HasCaret = true;
                    lineSelection.CaretPosition = caretPosition;
                    if (endLineSub == lineSub && fakeSelected && lineSelection.CaretPosition < 1.0f)
                        lineSelection.CaretPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
                }
            }
            else
            {
                if (lineSub == LineSub)
                {
                    float caretPosition = LineSub.GetCharPosition(caret.Position);
                    lineSelection.HasCaret = true;
                    lineSelection.CaretPosition = caretPosition;
                    if (endLineSub == lineSub && fakeSelected && lineSelection.CaretPosition < 1.0f)
                        lineSelection.CaretPosition = Singleton.FontManager.GetFontWhiteSpaceWidth();
                }
            }
        }
        return lineSelection;
    }
}