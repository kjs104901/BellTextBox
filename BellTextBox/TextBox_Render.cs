using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public abstract partial class TextBox
{
    public abstract RectSize GetCharRenderSize(char c);
    
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
        RenderText(new Vector2(150, 0), $"{_debugTextCoordinates.Row} {_debugTextCoordinates.Column}", FontStyle.DefaultFontStyle);
        RenderText(new Vector2(150, 20), KeyboardInput.ImeComposition, FontStyle.DefaultFontStyle);
    }
}