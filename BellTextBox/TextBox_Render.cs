using System.Numerics;
using Bell.Data;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    public float LineNumberWidth = 10.0f;
    public float FoldWidth = 10.0f;

    public void Render(Vector2 viewPos, Vector2 viewSize)
    {
        Singleton.TextBox = this;
        ProcessInput(viewPos, viewSize);

        FontManager.UpdateReferenceSize();
        FoldWidth = FontManager.GetFontReferenceWidth() * 2;

        _backend.RenderPage(PageSize, new Vector4(0.2f, 0.1f, 0.1f, 1.0f)); // TODO background color
        
        LineNumberWidth = StringPool<int>.Get(LineManager.Lines.Count).Sum(FontManager.GetFontWidth) + FontManager.GetFontReferenceWidth();

        int rowStart = GetRowIndex(_viewPos, -3);
        int rowEnd = GetRowIndex(_viewPos + _viewSize, 3);

        for (int i = rowStart; i <= rowEnd; i++)
        {
            if (LineManager.Rows.Count <= i)
                break;

            Row row = LineManager.Rows[i];

            var lineY = i * FontManager.GetLineHeight();
            var lineTextStartY = lineY + FontManager.GetLineHeightOffset();
            var lineEndY = (i + 1) * FontManager.GetLineHeight();
            var lineTextEndY = lineEndY - FontManager.GetLineHeightOffset();

            var lineStartX = LineNumberWidth + FoldWidth + row.IndentWidth;

            if (row.LineSelection.Selected)
            {
                _backend.RenderRectangle(new Vector2(lineStartX + row.LineSelection.SelectionStart, lineTextStartY),
                    new Vector2(lineStartX + row.LineSelection.SelectionEnd, lineTextEndY),
                    Theme.LineSelectedBackgroundColor.ToVector());
            }

            foreach (var textBlockRender in row.TextBlockRenders)
            {
                _backend.RenderText(
                    new Vector2(lineStartX + textBlockRender.PosX, lineTextStartY),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
            }

            foreach (var whiteSpaceRender in row.WhiteSpaceRenders)
            {
                if (whiteSpaceRender.C == ' ')
                {
                    _backend.RenderText(
                        new Vector2(lineStartX + whiteSpaceRender.PosX, lineTextStartY),
                        "·",
                        Theme.LineWhiteSpaceFontColor.ToVector());
                }
                else if (whiteSpaceRender.C == '\t')
                {
                    _backend.RenderLine(
                        new Vector2(lineStartX + whiteSpaceRender.PosX,
                            lineTextStartY),
                        new Vector2(
                            lineStartX + whiteSpaceRender.PosX +
                            _backend.GetCharWidth(' ') * 4, // TODO setting tab size
                            lineTextStartY),
                        Theme.LineWhiteSpaceFontColor.ToVector(),
                        1.0f);
                }
            }

            if (row.SubLine.WrapIndex == 0)
            {
                string lineIndex = StringPool<int>.Get(row.SubLine.LineCoordinates.Line.Index);
                float lineIndexWidth = lineIndex.Sum(FontManager.GetFontWidth);

                _backend.RenderText(new Vector2(LineNumberWidth - lineIndexWidth, lineTextStartY),
                    lineIndex,
                    Theme.DefaultFontColor.ToVector());
            }
            
            if (Folding.None != row.SubLine.LineCoordinates.Line.Folding)
            {
                _backend.RenderText(new Vector2(LineNumberWidth, lineTextStartY),
                    row.SubLine.LineCoordinates.Line.Folding.Folded ? " >" : " V",
                    Theme.DefaultFontColor.ToVector());
            }

            if (row.LineSelection.HasCaret)
            {
                _backend.RenderLine(
                    new Vector2(
                        lineStartX + row.LineSelection.CaretPosition - 1.0f,
                        lineTextStartY),
                    new Vector2(
                        lineStartX + row.LineSelection.CaretPosition - 1.0f,
                        lineTextEndY),
                    Theme.DefaultFontColor.ToVector(),
                    2.0f);

                _backend.RenderText(new Vector2(
                        lineStartX + row.LineSelection.CaretPosition,
                        lineTextStartY),
                    _imeComposition, Theme.DefaultFontColor.ToVector());
            }

            if (row.LineSelection.HasCaretAnchor)
            {
                _backend.RenderLine(
                    new Vector2(
                        lineStartX + row.LineSelection.CaretAnchorPosition - 1.0f,
                        lineTextStartY),
                    new Vector2(
                        lineStartX + row.LineSelection.CaretAnchorPosition - 1.0f,
                        lineTextEndY),
                    Theme.LineCommentFontColor.ToVector(),
                    2.0f);
            }
        }
    }
}