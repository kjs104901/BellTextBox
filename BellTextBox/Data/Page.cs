using Bell.Coordinates;
using Bell.Render;

namespace Bell.Data;

public class Page
{
    private readonly TextBox _textBox;
    
    public readonly Text Text;

    public PageRender Render => _renderCache.Get();
    private readonly Cache<PageRender> _renderCache;
    
    // View
    private ViewCoordinates _viewStart = new ();
    private ViewCoordinates _viewEnd = new ();

    private int _viewLineStart = 0;
    private int _viewLineEnd = 0;
    private bool _viewLineDirty = false;

    public Page(TextBox textBox)
    {
        _textBox = textBox;
        Text = new Text(_textBox);
        
        _renderCache = new Cache<PageRender>(new PageRender(), UpdateRender);
    }

    public void SetText(string text)
    {
        Text.SetText(text);
        _renderCache.SetDirty();
    }
    
    public void UpdateView(ViewCoordinates start, ViewCoordinates end)
    {
        _viewStart = start;
        _viewEnd = end;
        
        _viewLineDirty = true;
    }

    private PageRender UpdateRender(PageRender render)
    {
        render.Size.Width = 500; //TODO find width from render
        render.Size.Height = Text.LineViews.Count * _textBox.FontSizeManager.GetFontHeight();
        return render;
    }
}