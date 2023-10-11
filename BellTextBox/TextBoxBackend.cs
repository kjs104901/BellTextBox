using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public abstract class TextBoxBackend
{
    public KeyboardInput KeyboardInput;
    public MouseInput MouseInput;
    public ViewInput ViewInput;

    public PageRender PageRender;
    
    public abstract void Input();
    public abstract void StartTextBox();
    public abstract void EndTextBox();
    public abstract void StartPage();
    public abstract void EndPage();

    public abstract void SetClipboard(string text);
    public abstract string GetClipboard();
    
    public abstract RectSize GetCharRenderSize(char c);
    public abstract void RenderText(Vector2 pos, string text, FontStyle fontStyle);
}