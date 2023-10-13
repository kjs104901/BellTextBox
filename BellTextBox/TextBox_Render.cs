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
            float width = 0.0f;
            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                RenderText(
                    new Vector2(LineNumberWidth + FoldWidth + lineRender.PosX + width, lineRender.PosY),
                    textBlockRender.Text,
                    textBlockRender.ColorStyle.ToVector());
                width += textBlockRender.Width;
            }
        }

        foreach (Caret caret in CaretManager.Carets)
        {
            PageCoordinates caretInPage = CoordinatesManager.TextToPage(caret.Position);
            ViewCoordinates caretInView = CoordinatesManager.PageToView(caretInPage);

            RenderLine(new Vector2(caretInView.X - 1, caretInView.Y),
                new Vector2(caretInView.X - 1, caretInView.Y + FontSizeManager.GetFontSize()),
                Theme.DefaultFontColor.ToVector(),
                2.0f);
            
            RenderText(new Vector2(caretInView.X, caretInView.Y), KeyboardInput.ImeComposition, Theme.DefaultFontColor.ToVector());
        }
    }
}