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
        ThreadLocal.TextBox = this;
        ProcessInput(viewPos, viewSize);
        
        UpdateReferenceSize();
        FoldWidth = GetFontReferenceWidth() * 2;

        _backend.RenderPage(PageSize, new Vector4(0.2f, 0.1f, 0.1f, 1.0f)); // TODO background color
        
        
        float lineNumberWidthMax = 0.0f;
        
        for (int i = _pageStart.RowIndex; i <= _pageEnd.RowIndex; i++)
        {
            if (Rows.Count <= i)
                break;
            
            SubLine subLine = Rows[i];

            var lineY = subLine.Row * GetFontHeight();
            var lineTextStartY = lineY + GetFontHeightOffset();
            var lineEndY = (subLine.Row + 1) * GetFontHeight();
            var lineTextEndY = lineEndY - GetFontHeightOffset();

            var lineStartX = LineNumberWidth + FoldWidth + subLine.WrapIndentWidth;

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
                        new Vector2(lineStartX + whiteSpaceRender.PosX + _backend.GetCharWidth(' ') * 4, // TODO setting tab size
                            lineTextStartY),
                        Theme.LineWhiteSpaceFontColor.ToVector(),
                        1.0f);
                }
            }

            if (subLine.WrapIndex == 0)
            {
                float lineNumberWidth = 0.0f;
                string lineIndex = StringPool<int>.Get(subLine.LineIndex);

                foreach (char c in lineIndex)
                {
                    lineNumberWidth += GetFontWidth(c);
                }

                lineNumberWidthMax = Math.Max(lineNumberWidthMax, lineNumberWidth);

                _backend.RenderText(new Vector2(LineNumberWidth - lineNumberWidth, lineTextStartY),
                    lineIndex,
                    Theme.DefaultFontColor.ToVector());
            }

            if (subLine.Folding != null)
            {
                _backend.RenderText(new Vector2(LineNumberWidth, lineTextStartY),
                    subLine.Folding.Folded ? " >" : " V",
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

        LineNumberWidth = lineNumberWidthMax + 30.0f; // TODO setting padding option

        foreach (Caret caret in Carets)
        {
            Vector2 caretInPage = TextToPage(caret.Position);
            Vector2 caretInView = PageToView(caretInPage);

            RenderLine(new Vector2(caretInView.X - 1, caretInView.Y + GetFontHeightOffset()),
                new Vector2(caretInView.X - 1, caretInView.Y + GetFontHeight() - GetFontHeightOffset()),
                Theme.DefaultFontColor.ToVector(),
                2.0f);

            Vector2 selectionInPage = TextToPage(caret.Selection);
            Vector2 selectionInView = PageToView(selectionInPage);

            RenderLine(new Vector2(selectionInView.X - 1, selectionInView.Y + GetFontHeightOffset()),
                new Vector2(selectionInView.X - 1, selectionInView.Y + GetFontHeight() - GetFontHeightOffset()),
                Theme.LineCommentFontColor.ToVector(),
                2.0f);

            RenderText(caretInView, _imeComposition, Theme.DefaultFontColor.ToVector());
        }
    }
}