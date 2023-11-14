using System.Numerics;
using Bell.Data;
using Bell.Languages;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    internal float LineNumberWidth = 10.0f;
    internal float FoldWidth = 10.0f;

    public void Render(Vector2 viewPos, Vector2 viewSize)
    {
        Singleton.TextBox = this;
        ProcessInput(viewPos, viewSize);

        FontManager.UpdateReferenceSize();
        FoldWidth = FontManager.GetFontReferenceWidth() * 2;

        Backend.RenderPage(PageSize, new Vector4(0.2f, 0.1f, 0.1f, 1.0f)); // TODO background color

        LineNumberWidth = (StringPool<int>.Get(LineManager.Lines.Count).Length + 1) * FontManager.GetFontNumberWidth();

        LineManager.UpdateLanguage();

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
                    LineSelectedBackgroundColor.ToVector());
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
                    
                    Backend.RenderText(new Vector2(lineStartX + currPosX, lineTextStartY),
                        StringPool<char>.Get(rowChar), charColor.ToVector());
                    
                    if (rowChar == ' ')
                    {
                        Backend.RenderText(
                            new Vector2(lineStartX + currPosX, lineTextStartY),
                            "·",
                            charColor.ToVector());
                    }
                    else if (rowChar == '\t')
                    {
                        Backend.RenderLine(
                            new Vector2(lineStartX + currPosX, lineTextStartY),
                            new Vector2(lineStartX + currPosX + Backend.GetCharWidth(' ') * 4, // TODO setting tab size
                                lineTextStartY),
                            charColor.ToVector(),
                            1.0f);
                    }

                    currPosX += rowCharWidth;
                }

                if (row.LineSub.Coordinates.LineSubIndex == 0)
                {
                    string lineIndex = StringPool<int>.Get(line.Index);
                    float lineIndexWidth = lineIndex.Sum(FontManager.GetFontWidth);

                    Backend.RenderText(new Vector2(LineNumberWidth - lineIndexWidth, lineTextStartY),
                        lineIndex,
                        UiTextColor.ToVector());
                }

                if (Folding.None != line.Folding)
                {
                    Backend.RenderText(new Vector2(LineNumberWidth, lineTextStartY),
                        line.Folding.Folded ? " >" : " V",
                        UiTextColor.ToVector());
                }
            }

            //foreach (float caretAnchorPosition in row.RowSelection.CaretAnchorPositions)
            //{
            //    Backend.RenderLine(
            //        new Vector2(lineStartX + caretAnchorPosition - 1.0f, lineTextStartY),
            //        new Vector2(lineStartX + caretAnchorPosition - 1.0f, lineTextEndY),
            //        Language.LineCommentFontColor.ToVector(),
            //        2.0f);
            //}

            foreach (float caretPosition in row.RowSelection.CaretPositions)
            {
                Backend.RenderLine(
                    new Vector2(lineStartX + caretPosition - 1.0f, lineTextStartY),
                    new Vector2(lineStartX + caretPosition - 1.0f, lineTextEndY),
                    CaretColor.ToVector(),
                    2.0f);

                Backend.RenderText(new Vector2(lineStartX + caretPosition, lineTextStartY),
                    _imeComposition, ImeInputColor.ToVector());
            }
        }
    }
}