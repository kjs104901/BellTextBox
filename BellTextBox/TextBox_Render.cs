using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;

namespace Bell;

public abstract partial class TextBox
{
    protected abstract void RenderText(Vector2 pos, string text, Vector4 color);
    protected abstract void RenderLine(Vector2 start, Vector2 end, Vector4 color, float thickness);
    protected abstract void RenderRectangle(Vector2 start, Vector2 end, Vector4 color);

    protected abstract void SetMouseCursor(MouseCursor mouseCursor);

    public float LineNumberWidth = 10.0f;
    public float FoldWidth = 10.0f;

    protected void Render()
    {
        UpdateReferenceSize();
        FoldWidth = GetFontReferenceWidth() * 2;

        float lineNumberWidthMax = 0.0f;
        foreach (LineRender lineRender in Text.ShowLineRenders)
        {
            if (_caretChanged || lineRender.CaretSet == false)
            {
                lineRender.SetCarets(Carets);
            }

            var lineY = lineRender.Row * GetFontHeight();
            var lineTextStartY = lineY + GetFontHeightOffset();
            var lineEndY = (lineRender.Row + 1) * GetFontHeight();
            var lineTextEndY = lineEndY - GetFontHeightOffset();

            var lineStartX = LineNumberWidth + FoldWidth + lineRender.TabWidth;

            if (lineRender.Selected)
            {
                RenderRectangle(new Vector2(lineStartX + lineRender.SelectionStart, lineTextStartY),
                    new Vector2(lineStartX + lineRender.SelectionEnd, lineTextEndY),
                    Theme.LineSelectedBackgroundColor.ToVector());
            }

            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                RenderText(
                    new Vector2(lineStartX + textBlockRender.PosX, lineTextStartY),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
            }

            foreach (WhiteSpaceRender whiteSpaceRender in lineRender.WhiteSpaceRenders)
            {
                if (whiteSpaceRender.C == ' ')
                {
                    RenderText(
                        new Vector2(lineStartX + whiteSpaceRender.PosX, lineTextStartY),
                        "·",
                        Theme.LineWhiteSpaceFontColor.ToVector());
                }
                else if (whiteSpaceRender.C == '\t')
                {
                    RenderLine(
                        new Vector2(lineStartX + whiteSpaceRender.PosX,
                            lineTextStartY),
                        new Vector2(lineStartX + whiteSpaceRender.PosX + GetCharWidth(' ') * 4, // TODO setting tab size
                            lineTextStartY),
                        Theme.LineWhiteSpaceFontColor.ToVector(),
                        1.0f);
                }
            }

            if (lineRender.WrapIndex == 0)
            {
                float lineNumberWidth = 0.0f;
                foreach (char c in lineRender.LineIndex.ToString())
                {
                    lineNumberWidth += GetFontWidth(c);
                }

                lineNumberWidthMax = Math.Max(lineNumberWidthMax, lineNumberWidth);

                RenderText(new Vector2(LineNumberWidth - lineNumberWidth, lineTextStartY),
                    lineRender.LineIndex.ToString(),
                    Theme.DefaultFontColor.ToVector());
            }

            RenderText(new Vector2(LineNumberWidth, lineTextStartY),
                "#",
                Theme.DefaultFontColor.ToVector());


            if (lineRender.CaretPosition)
            {
                RenderLine(
                    new Vector2(
                        lineStartX + lineRender.CaretPositionPosition - 1.0f,
                        lineTextStartY),
                    new Vector2(
                        lineStartX + lineRender.CaretPositionPosition - 1.0f,
                        lineTextEndY),
                    Theme.DefaultFontColor.ToVector(),
                    2.0f);
            }

            if (lineRender.CaretSelection)
            {
                RenderLine(
                    new Vector2(
                        lineStartX + lineRender.CaretSelectionPosition - 1.0f,
                        lineTextStartY),
                    new Vector2(
                        lineStartX + lineRender.CaretSelectionPosition - 1.0f,
                        lineTextEndY),
                    Theme.LineCommentFontColor.ToVector(),
                    2.0f);
            }
        }

        LineNumberWidth = lineNumberWidthMax + 30.0f; // TODO setting padding option

        //foreach (Caret caret in Carets)
        //{
        //    Vector2 caretInPage = TextToPage(caret.Position);
        //    Vector2 caretInView = PageToView(caretInPage);
//
        //    RenderLine(new Vector2(caretInView.X - 1, caretInView.Y + GetFontHeightOffset()),
        //        new Vector2(caretInView.X - 1, caretInView.Y + GetFontHeight() - GetFontHeightOffset()),
        //        Theme.DefaultFontColor.ToVector(),
        //        2.0f);
//
        //    Vector2 selectionInPage = TextToPage(caret.Selection);
        //    Vector2 selectionInView = PageToView(selectionInPage);
//
        //    RenderLine(new Vector2(selectionInView.X - 1, selectionInView.Y + GetFontHeightOffset()),
        //        new Vector2(selectionInView.X - 1, selectionInView.Y + GetFontHeight() - GetFontHeightOffset()),
        //        Theme.LineCommentFontColor.ToVector(),
        //        2.0f);
//
        //    RenderText(caretInView, _imeComposition, Theme.DefaultFontColor.ToVector());
        //}
    }
}