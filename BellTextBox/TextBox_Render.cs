using System.Diagnostics;
using System.Numerics;
using Bell.Data;
using Bell.Languages;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    internal float LineNumberWidth = 10.0f;
    internal float FoldWidth = 10.0f;

    internal readonly Stopwatch CaretBlinkStopwatch = new();

    internal int LinesPerPage => (int)(_viewSize.Y / FontManager.GetLineHeight());

    public void Render(Vector2 viewPos, Vector2 viewSize)
    {
        Singleton.TextBox = this;
        ProcessInput(viewPos, viewSize);

        FontManager.UpdateReferenceSize();
        FoldWidth = FontManager.GetFontReferenceWidth() * 2;

        Backend.RenderPage(PageSize, PageBackgroundColor.ToVector());

        LineNumberWidth = (StringPool<int>.Get(LineManager.Lines.Count).Length + 1) * FontManager.GetFontNumberWidth();

        LineManager.UpdateLanguageToken();

        int rowStart = GetRowIndex(_viewPos, -3);
        int rowEnd = GetRowIndex(_viewPos + _viewSize, 3);

        for (int i = rowStart; i <= rowEnd; i++)
        {
            if (RowManager.Rows.Count <= i)
                break;

            Row row = RowManager.Rows[i];

            var lineY = i * FontManager.GetLineHeight();
            var lineTextStartY = lineY + FontManager.GetLineHeightOffset();
            var lineEndY = (i + 1) * FontManager.GetLineHeight();
            var lineTextEndY = lineEndY - FontManager.GetLineHeightOffset();

            var lineStartX = LineNumberWidth + FoldWidth + row.LineSub.IndentWidth;

            if (row.RowSelection.Selected)
            {
                Backend.RenderRectangle(new Vector2(lineStartX + row.RowSelection.SelectionStart, lineTextStartY),
                    new Vector2(lineStartX + row.RowSelection.SelectionEnd, lineTextEndY),
                    SelectedBackgroundColor.ToVector());
            }

            if (LineManager.GetLine(row.LineSub.Coordinates.LineIndex, out Line line))
            {
                float currPosX = 0.0f;

                for (int j = 0; j < row.LineSub.Chars.Count; j++)
                {
                    char rowChar = row.LineSub.Chars[j];
                    int rowCharIndex = row.LineSub.Coordinates.CharIndex + j;
                    float rowCharWidth = row.LineSub.CharWidths[j];

                    ColorStyle charColor = line.GetColorStyle(rowCharIndex);

                    foreach (Coordinates caretPosition in row.RowSelection.CaretPositions)
                    {
                        if (rowCharIndex == caretPosition.CharIndex)
                            DrawImeComposition(lineStartX, lineTextStartY, lineTextEndY, ref currPosX);
                    }

                    Backend.RenderText(new Vector2(lineStartX + currPosX, lineTextStartY),
                        StringPool<char>.Get(rowChar), charColor.ToVector());

                    if (ShowingWhitespace)
                    {
                        if (rowChar == ' ')
                        {
                            Backend.RenderText(
                                new Vector2(lineStartX + currPosX, lineTextStartY),
                                "·", WhiteSpaceFontColor.ToVector());
                        }
                        else if (rowChar == '\t')
                        {
                            Backend.RenderLine(
                                new Vector2(
                                    lineStartX + currPosX,
                                    lineY + FontManager.GetLineHeight() / 2.0f),
                                new Vector2(
                                    lineStartX + currPosX - FontManager.GetFontWhiteSpaceWidth() +
                                    FontManager.GetFontTabWidth(),
                                    lineY + FontManager.GetLineHeight() / 2.0f),
                                WhiteSpaceFontColor.ToVector(), 2.0f);
                        }
                    }

                    currPosX += rowCharWidth;
                }

                // When the Caret is end of chars
                foreach (Coordinates caretPosition in row.RowSelection.CaretPositions)
                {
                    if (row.LineSub.Chars.Count == caretPosition.CharIndex)
                        DrawImeComposition(lineStartX, lineTextStartY, lineTextEndY, ref currPosX);
                }

                if (row.LineSub.Coordinates.LineSubIndex == 0)
                {
                    string lineIndex = StringPool<int>.Get(line.Index);
                    float lineIndexWidth = lineIndex.Sum(FontManager.GetFontWidth);

                    Backend.RenderText(new Vector2(LineNumberWidth - lineIndexWidth, lineTextStartY),
                        lineIndex,
                        UiTextColor.ToVector());
                }

                if (Folding.None != line.Folding && row.LineSub.Coordinates.LineSubIndex == 0)
                {
                    float midX = LineNumberWidth + (FoldWidth / 2.0f);
                    float midY = (lineTextStartY + lineTextEndY) / 2.0f;

                    float barLength = (FoldWidth) / 6.0f;

                    if (line.Folding.Folded)
                    {
                        Backend.RenderLine(new Vector2(midX - barLength, midY - barLength),
                            new Vector2(midX, midY),
                            UiTextColor.ToVector(), 2.0f);

                        Backend.RenderLine(new Vector2(midX - barLength, midY + barLength),
                            new Vector2(midX, midY),
                            UiTextColor.ToVector(), 2.0f);
                    }
                    else
                    {
                        Backend.RenderLine(new Vector2(midX - barLength, midY - barLength),
                            new Vector2(midX, midY),
                            UiTextColor.ToVector(), 2.0f);

                        Backend.RenderLine(new Vector2(midX + barLength, midY - barLength),
                            new Vector2(midX, midY),
                            UiTextColor.ToVector(), 2.0f);
                    }
                }
            }

            if (CaretBlinkStopwatch.ElapsedMilliseconds < 1000 ||
                CaretBlinkStopwatch.ElapsedMilliseconds % 1000 < 500)
            {
                foreach (Coordinates caretPosition in row.RowSelection.CaretPositions)
                {
                    float caretX = row.LineSub.GetCharPosition(caretPosition);
                    Backend.RenderLine(
                        new Vector2(lineStartX + caretX - 1.0f, lineTextStartY),
                        new Vector2(lineStartX + caretX - 1.0f, lineTextEndY),
                        CaretColor.ToVector(),
                        2.0f);
                }
            }
        }
    }

    private void DrawImeComposition(float lineStartX, float lineTextStartY, float lineTextEndY, ref float currPosX)
    {
        if (_imeComposition is not { Length: > 0 })
            return;

        Backend.RenderText(new Vector2(lineStartX + currPosX, lineTextStartY), _imeComposition,
            ImeInputColor.ToVector());

        float compositionWidth = FontManager.GetFontWidth(_imeComposition);

        Backend.RenderLine(new Vector2(lineStartX + currPosX, lineTextEndY),
            new Vector2(lineStartX + currPosX + compositionWidth, lineTextEndY),
            ImeInputColor.ToVector(), 1.0f);

        currPosX += compositionWidth;
    }
}