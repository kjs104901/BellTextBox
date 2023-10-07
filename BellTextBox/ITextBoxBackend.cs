using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Render;

namespace Bell;

public interface ITextBoxBackend
{
    public void SetClipboard(string text);
    public string GetClipboard();

    public void Render(Action<KeyboardInput, MouseInput, ViewInput> inputAction, PageRender pageRender, List<LineRender> lineRenders);
    public RectSize GetRenderSize(char c);
}