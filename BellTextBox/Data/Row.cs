using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

internal class Row
{
    internal RowSelection RowSelection => RowSelectionCache.Get();
    internal readonly Cache<RowSelection> RowSelectionCache;

    internal readonly LineSub LineSub;

    internal Row(LineSub lineSub)
    {
        LineSub = lineSub;

        RowSelectionCache = new("Row Selection", new RowSelection()
            {
                CaretPositions = new(),
                CaretAnchorPositions = new()
            },
            UpdateRowSelection);
    }

    private RowSelection UpdateRowSelection(RowSelection rowSelection)
    {
        rowSelection.Selected = false;
        rowSelection.SelectionStart = 0.0f;
        rowSelection.SelectionEnd = 0.0f;
        
        rowSelection.CaretPositions.Clear();
        rowSelection.CaretAnchorPositions.Clear();

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

            caret.GetSorted(out Coordinates start, out Coordinates end);
            if (false == LineManager.GetLineSub(start, out LineSub startLineSub) ||
                false == LineManager.GetLineSub(end, out LineSub endLineSub))
            {
                Logger.Error("UpdateLineSelection: failed to get line");
                continue;
            }

            if (caret.HasSelection)
            {
                if (startLineSub == LineSub)
                {
                    float startPosition = LineSub.GetCharPosition(start);
                    if (endLineSub == LineSub)
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        rowSelection.SelectionStart = startPosition;
                        rowSelection.SelectionEnd = endPosition;
                        rowSelection.Selected = true;
                    }
                    else if (endLineSub.IsBiggerThan(LineSub))
                    {
                        rowSelection.SelectionStart = startPosition;
                        rowSelection.SelectionEnd = LineSub.CharWidths.Sum();
                        if (rowSelection.SelectionEnd < 1.0f)
                        {
                            rowSelection.SelectionEnd = FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }

                        rowSelection.Selected = true;
                    }
                }
                else if (LineSub.IsBiggerThan(startLineSub))
                {
                    if (endLineSub == LineSub)
                    {
                        float endPosition = LineSub.GetCharPosition(end);
                        rowSelection.SelectionStart = 0.0f;
                        rowSelection.SelectionEnd = endPosition;
                        rowSelection.Selected = true;
                    }
                    else if (endLineSub.IsBiggerThan(LineSub))
                    {
                        rowSelection.SelectionStart = 0.0f;
                        rowSelection.SelectionEnd = LineSub.CharWidths.Sum();
                        if (rowSelection.SelectionEnd < 1.0f)
                        {
                            rowSelection.SelectionEnd = FontManager.GetFontWhiteSpaceWidth();
                            fakeSelected = true;
                        }

                        rowSelection.Selected = true;
                    }
                }
            }

            if (anchorLineSub == LineSub)
            {
                float anchorPosition = LineSub.GetCharPosition(caret.AnchorPosition);
                if (endLineSub == anchorLineSub && fakeSelected && anchorPosition < 1.0f)
                    anchorPosition = FontManager.GetFontWhiteSpaceWidth();
                rowSelection.CaretAnchorPositions.Add(anchorPosition);
            }

            if (lineSub == LineSub)
            {
                float caretPosition = LineSub.GetCharPosition(caret.Position);
                if (endLineSub == lineSub && fakeSelected && caretPosition < 1.0f)
                    caretPosition = FontManager.GetFontWhiteSpaceWidth();
                rowSelection.CaretPositions.Add(caretPosition);
            }
        }

        return rowSelection;
    }
}

internal struct RowSelection
{
    internal bool Selected;
    internal float SelectionStart;
    internal float SelectionEnd;

    internal List<float> CaretPositions;
    internal List<float> CaretAnchorPositions;
}