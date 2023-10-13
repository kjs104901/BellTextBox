using System.Numerics;
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

        if (CaretManager.GetCaretForDebug(out var caret))
        {
            PageCoordinates caretInPage = CoordinatesManager.TextToPage(caret.Position);
            ViewCoordinates caretInView = CoordinatesManager.PageToView(caretInPage);

            RenderLine(new Vector2(caretInView.X - 1, caretInView.Y),
                new Vector2(caretInView.X - 1, caretInView.Y + FontSizeManager.GetFontSize()),
                Theme.DefaultFontColor.ToVector(),
                2.0f);
        }


        var testString = "}}}}}}}}}}";
        RenderText(new Vector2(150, 50), testString, Theme.DefaultFontColor.ToVector());
        int i = 0;
        foreach (var testChar in testString)
        {
            RenderText(new Vector2(150 + FontSizeManager.GetFontWidth(testChar) * i, 80), testChar.ToString(),
                Theme.DefaultFontColor.ToVector());
            i++;
        }

        testString = "AAAAAAAAAA";
        RenderText(new Vector2(150, 110), testString, Theme.DefaultFontColor.ToVector());
        i = 0;
        foreach (var testChar in testString)
        {
            RenderText(new Vector2(150 + FontSizeManager.GetFontWidth(testChar) * i, 140), testChar.ToString(),
                Theme.DefaultFontColor.ToVector());
            i++;
        }

        testString = "||||||||||";
        RenderText(new Vector2(150, 170), testString, Theme.DefaultFontColor.ToVector());
        i = 0;
        foreach (var testChar in testString)
        {
            RenderText(new Vector2(150 + FontSizeManager.GetFontWidth(testChar) * i, 200), testChar.ToString(),
                Theme.DefaultFontColor.ToVector());
            i++;
        }

        RenderText(new Vector2(150, 0), $"{_debugTextCoordinates.Row} {_debugTextCoordinates.Column}",
            Theme.DefaultFontColor.ToVector());
        RenderText(new Vector2(150, 20), KeyboardInput.ImeComposition, Theme.DefaultFontColor.ToVector());
    }
}