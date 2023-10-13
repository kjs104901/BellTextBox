using System.Numerics;
using Bell.Carets;
using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public abstract partial class TextBox
{
    public abstract float GetCharWidth(char c);
    public abstract float GetFontSize();

    protected abstract void RenderText(Vector2 pos, string text, Vector4 color);
    protected abstract void RenderLine(Vector2 start, Vector2 end, Vector4 color, float thickness);

    public abstract void SetClipboard(string text);
    public abstract string GetClipboard();

    public abstract void SetMouseCursor(MouseCursor mouseCursor);

    protected void Render()
    {
        FontSizeManager.UpdateReferenceSize();

        foreach (LineRender lineRender in Page.LineRenders)
        {
            foreach (Caret caret in CaretManager.Carets)
            {
                if (caret.HasSelection)
                {
                    //caret.Position
                }
            }

            float width = 0.0f;
            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                RenderText(
                    new Vector2(LineNumberWidth + FoldWidth + width,
                        lineRender.RenderIndex * FontSizeManager.GetFontSize()),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
                width += textBlockRender.Width;
            }
        }

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