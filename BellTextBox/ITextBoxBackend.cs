using Bell.Coordinates;
using Bell.Data;
using Bell.Render;

namespace Bell;

public interface ITextBoxBackend
{
    public void SetClipboard(string text);
    public string GetClipboard();

    public void Render(TextBox textBox, List<LineRender> lineRenders);
    public RectSize GetRenderSize(char c);
}