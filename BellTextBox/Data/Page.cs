using Bell.Coordinates;
using Bell.Render;

namespace Bell.Data;

public class Page
{
    private readonly TextBox _textBox;
    
    public readonly Text Text;

    public PageRender Render => _renderCache.Get();
    private readonly Cache<PageRender> _renderCache;
    
    public List<LineRender> LineRenders => _lineRendersCache.Get();
    private readonly Cache<List<LineRender>> _lineRendersCache;
    
    // View
    private ViewCoordinates _viewStart;
    private ViewCoordinates _viewEnd;

    public Page(TextBox textBox)
    {
        _textBox = textBox;
        Text = new Text(_textBox);
        
        _renderCache = new Cache<PageRender>(new PageRender(), UpdateRender);
        _lineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateLineRenders);
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
        
        _lineRendersCache.SetDirty();
    }
    
    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();
        // TODO find line view range from _viewStart, _viewEnd
        int i = 0;
        foreach (LineView lineView in Text.LineViews)
        {
            var lineRender = Text.Lines[lineView.LineIndex].GetLineRender(lineView.RenderIndex);

            lineRender.PosX = 0;
            lineRender.PosY = (i++) * _textBox.FontSizeManager.GetFontHeight();
            
            lineRenders.Add(lineRender);
        }
        return lineRenders;
    }

    private PageRender UpdateRender(PageRender render)
    {
        render.Size.Width = 500; //TODO find width from render
        render.Size.Height = Text.LineViews.Count * _textBox.FontSizeManager.GetFontHeight();
        return render;
    }
}