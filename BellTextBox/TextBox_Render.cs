using System.Numerics;
using Bell.Carets;
using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;

namespace Bell;

public abstract partial class TextBox
{
    public abstract float GetCharWidth(char c);
    public abstract float GetFontSize();

    protected abstract void RenderText(Vector2 pos, string text, Vector4 color);
    protected abstract void RenderLine(Vector2 start, Vector2 end, Vector4 color, float thickness);

    public abstract void SetClipboard(string text);
    public abstract string GetClipboard();

    protected abstract void SetMouseCursor(MouseCursor mouseCursor);
    
    public float LineNumberWidthMax = 10.0f;
    public float FoldWidth = 10.0f;

    protected void Render()
    {
        FontSizeManager.UpdateReferenceSize();
        FoldWidth = FontSizeManager.GetFontReferenceWidth() * 2;

        float lineNumberWidthMax = 0.0f;
        foreach (LineRender lineRender in Page.LineRenders)
        {
            foreach (Caret caret in CaretManager.Carets)
            {
                if (caret.HasSelection)
                {
                    //TODO draw selection 
                }
            }

            var lineY = lineRender.Row * FontSizeManager.GetFontSize();

            float width = 0.0f;
            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                RenderText(
                    new Vector2(LineNumberWidthMax + FoldWidth + width, lineY),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
                width += textBlockRender.Width;
            }

            if (lineRender.WrapIndex == 0)
            {
                float lineNumberWidth = 0.0f;
                foreach (char c in lineRender.LineIndex.ToString())
                {
                    lineNumberWidth += FontSizeManager.GetFontWidth(c);
                }
                lineNumberWidthMax = Math.Max(lineNumberWidthMax, lineNumberWidth);
                
                RenderText(new Vector2(LineNumberWidthMax - lineNumberWidth, lineY),
                    lineRender.LineIndex.ToString(),
                    Theme.DefaultFontColor.ToVector());
            }
            
            RenderText(new Vector2(LineNumberWidthMax, lineY),
                "#",
                Theme.DefaultFontColor.ToVector());
        }
        LineNumberWidthMax = lineNumberWidthMax + 30.0f; // TODO setting padding option

        foreach (Caret caret in CaretManager.Carets)
        {
            Vector2 caretInPage = CoordinatesManager.TextToPage(caret.Position);
            Vector2 caretInView = CoordinatesManager.PageToView(caretInPage);

            RenderLine(new Vector2(caretInView.X - 1, caretInView.Y),
                new Vector2(caretInView.X - 1, caretInView.Y + FontSizeManager.GetFontSize()),
                Theme.DefaultFontColor.ToVector(),
                2.0f);

            Vector2 selectionInPage = CoordinatesManager.TextToPage(caret.Selection);
            Vector2 selectionInView = CoordinatesManager.PageToView(selectionInPage);

            RenderLine(new Vector2(selectionInView.X - 1, selectionInView.Y),
                new Vector2(selectionInView.X - 1, selectionInView.Y + FontSizeManager.GetFontSize()),
                Theme.LineCommentFontColor.ToVector(),
                2.0f);

            RenderText(caretInView, KeyboardInput.ImeComposition, Theme.DefaultFontColor.ToVector());
        }
    }
}