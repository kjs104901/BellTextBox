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

        for (int i = _rowStart; i <= _rowEnd; i++)
        {
            if (LineManager.Rows.Count <= i)
                break;

            SubLine subLine = LineManager.Rows[i];

            var lineY = i * FontManager.GetLineHeight();
            var lineTextStartY = lineY + FontManager.GetLineHeightOffset();
            var lineEndY = (i + 1) * FontManager.GetLineHeight();
            var lineTextEndY = lineEndY - FontManager.GetLineHeightOffset();

            var lineStartX = LineNumberWidth + FoldWidth + subLine.IndentWidth;

            if (subLine.LineSelection.Selected)
            {
                _backend.RenderRectangle(new Vector2(lineStartX + subLine.LineSelection.SelectionStart, lineTextStartY),
                    new Vector2(lineStartX + subLine.LineSelection.SelectionEnd, lineTextEndY),
                    Theme.LineSelectedBackgroundColor.ToVector());
            }

            foreach (var textBlockRender in subLine.TextBlockRenders)
            {
                _backend.RenderText(
                    new Vector2(lineStartX + textBlockRender.PosX, lineTextStartY),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
            }

            foreach (var whiteSpaceRender in subLine.WhiteSpaceRenders)
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

            if (subLine.WrapIndex == 0)
            {
                string lineIndex = StringPool<int>.Get(subLine.LineCoordinates.Line.Index);
                float lineIndexWidth = lineIndex.Sum(FontManager.GetFontWidth);

                _backend.RenderText(new Vector2(LineNumberWidth - lineIndexWidth, lineTextStartY),
                    lineIndex,
                    Theme.DefaultFontColor.ToVector());
            }
            
            if (Folding.None != subLine.LineCoordinates.Line.Folding)
            {
                _backend.RenderText(new Vector2(LineNumberWidth, lineTextStartY),
                    subLine.LineCoordinates.Line.Folding.Folded ? " >" : " V",
                    Theme.DefaultFontColor.ToVector());
            }

            if (subLine.LineSelection.HasCaret)
            {
                _backend.RenderLine(
                    new Vector2(
                        lineStartX + subLine.LineSelection.CaretPosition - 1.0f,
                        lineTextStartY),
                    new Vector2(
                        lineStartX + subLine.LineSelection.CaretPosition - 1.0f,
                        lineTextEndY),
                    Theme.DefaultFontColor.ToVector(),
                    2.0f);

                _backend.RenderText(new Vector2(
                        lineStartX + subLine.LineSelection.CaretPosition,
                        lineTextStartY),
                    _imeComposition, Theme.DefaultFontColor.ToVector());
            }

            if (subLine.LineSelection.HasCaretAnchor)
            {
                _backend.RenderLine(
                    new Vector2(
                        lineStartX + subLine.LineSelection.CaretAnchorPosition - 1.0f,
                        lineTextStartY),
                    new Vector2(
                        lineStartX + subLine.LineSelection.CaretAnchorPosition - 1.0f,
                        lineTextEndY),
                    Theme.LineCommentFontColor.ToVector(),
                    2.0f);
            }
        }
    }
}