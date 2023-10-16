using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;

namespace Bell;

public abstract partial class TextBox
{
    protected abstract void RenderText(Vector2 pos, string text, Vector4 color);
    protected abstract void RenderLine(Vector2 start, Vector2 end, Vector4 color, float thickness);

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
            foreach (Caret caret in Carets)
            {
                if (caret.HasSelection)
                {
                    //TODO draw selection 
                }
            }

            var lineY = lineRender.Row * GetFontHeight();
            var lineTextY = lineRender.Row * GetFontHeight() + GetFontHeightOffset();

            float width = 0.0f;
            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                RenderText(
                    new Vector2(LineNumberWidth + FoldWidth + width, lineTextY),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
                width += textBlockRender.Width;
            }

            if (lineRender.WrapIndex == 0)
            {
                float lineNumberWidth = 0.0f;
                foreach (char c in lineRender.LineIndex.ToString())
                {
                    lineNumberWidth += GetFontWidth(c);
                }
                lineNumberWidthMax = Math.Max(lineNumberWidthMax, lineNumberWidth);
                
                RenderText(new Vector2(LineNumberWidth - lineNumberWidth, lineTextY),
                    lineRender.LineIndex.ToString(),
                    Theme.DefaultFontColor.ToVector());
            }
            
            RenderText(new Vector2(LineNumberWidth, lineTextY),
                "#",
                Theme.DefaultFontColor.ToVector());
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