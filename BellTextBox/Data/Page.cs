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

    public bool ToPageCoordinates(ViewCoordinates viewCoordinates, out PageCoordinates coordinates, out bool isLine, out bool isMarker)
    {
        //TODO
        //view.X + X - HeaderWidth;
        //view.Y + Y;
        coordinates = new PageCoordinates(0, 0);
        isLine = false;
        isMarker = false;
        return true;
    }

    public int ToLineIndex(ViewCoordinates viewCoordinates, int offset = 0)
    {
        int index = (int)(viewCoordinates.Y / _textBox.FontSizeManager.GetFontHeight()) + offset;
        if (index < 0)
            index = 0;
        if (index >= Text.LineViews.Count)
            index = Text.LineViews.Count - 1;
        return index;
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
        
        int lineStart = ToLineIndex(_viewStart, -3);
        int lineEnd = ToLineIndex(_viewEnd, 3);

        for (int i = lineStart; i <= lineEnd; i++)
        {
            LineView lineView = Text.LineViews[i];
            var lineRender = Text.Lines[lineView.LineIndex].GetLineRender(lineView.RenderIndex);

            lineRender.PosX = 0;
            lineRender.PosY = i * _textBox.FontSizeManager.GetFontHeight();
            
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