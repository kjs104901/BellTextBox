using System.Numerics;
using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public abstract partial class TextBox
{
    public abstract Vector2 GetCharRenderSize(char c);

    protected abstract void RenderText(Vector2 pos, string text, FontStyle fontStyle);

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
                    textBlockRender.FontStyle);
                width += textBlockRender.Width;
            }
        }

        if (CaretManager.GetCaretForDebug(out var caret))
        {
            PageCoordinates caretInPage = CoordinatesManager.TextToPage(caret.Position);
            ViewCoordinates caretInView = CoordinatesManager.PageToView(caretInPage);

            RenderText(new Vector2(caretInView.X, caretInView.Y), "|", FontStyle.DefaultFontStyle);
        }


        var testString = "}}}}}}}}}}";
        RenderText(new Vector2(150, 50), testString, FontStyle.DefaultFontStyle);
        int i = 0;
        foreach (var testChar in testString)
        {
            RenderText(new Vector2(150 + FontSizeManager.GetFontWidth(testChar) * i, 80), testChar.ToString(), FontStyle.DefaultFontStyle);
            i++;
        }

        testString = "AAAAAAAAAA";
        RenderText(new Vector2(150, 110), testString, FontStyle.DefaultFontStyle);
        i = 0;
        foreach (var testChar in testString)
        {
            RenderText(new Vector2(150 + FontSizeManager.GetFontWidth(testChar) * i, 140), testChar.ToString(), FontStyle.DefaultFontStyle);
            i++;
        }
        
        RenderText(new Vector2(150, 0), $"{_debugTextCoordinates.Row} {_debugTextCoordinates.Column}",
            FontStyle.DefaultFontStyle);
        RenderText(new Vector2(150, 20), KeyboardInput.ImeComposition, FontStyle.DefaultFontStyle);
    }
}