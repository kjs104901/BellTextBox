using Bell.Coordinates;
using Bell.Render;

namespace Bell.Data;

public class Page : IDisposable
{
    private TextBox? _textBox;

    public PageRender Render => RenderCache.Get();
    public readonly Cache<PageRender> RenderCache;
    
    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;
    
    // View
    private ViewCoordinates _viewStart;
    private ViewCoordinates _viewEnd;

    public Page(TextBox? textBox)
    {
        _textBox = textBox;
        
        RenderCache = new Cache<PageRender>(new PageRender(), UpdateRender);
        LineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateLineRenders);
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
        if (null == _textBox || null == _textBox.Text || null == _textBox.FontSizeManager)
            return 0;
        
        int index = (int)(viewCoordinates.Y / _textBox.FontSizeManager.GetFontHeight()) + offset;
        if (index < 0)
            index = 0;
        if (index >= _textBox.Text.LineViews.Count)
            index = _textBox.Text.LineViews.Count - 1;
        return index;
    }
    
    public void UpdateView(ViewCoordinates start, ViewCoordinates end)
    {
        _viewStart = start;
        _viewEnd = end;
        
        LineRendersCache.SetDirty();
    }
    
    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        if (null == _textBox || null == _textBox.Text || null == _textBox.FontSizeManager)
            return lineRenders;
        
        lineRenders.Clear();
        
        int lineStart = ToLineIndex(_viewStart, -3);
        int lineEnd = ToLineIndex(_viewEnd, 3);

        for (int i = lineStart; i <= lineEnd; i++)
        {
            LineView lineView = _textBox.Text.LineViews[i];
            var lineRender = _textBox.Text.Lines[lineView.LineIndex].GetLineRender(lineView.RenderIndex);

            lineRender.PosX = 0;
            lineRender.PosY = i * _textBox.FontSizeManager.GetFontHeight();
            
            lineRenders.Add(lineRender);
        }
        
        return lineRenders;
    }

    private PageRender UpdateRender(PageRender render)
    {
        if (null == _textBox || null == _textBox.Text || null == _textBox.FontSizeManager)
            return render;
        
        render.Size.Width = 500; //TODO find width from render
        render.Size.Height = _textBox.Text.LineViews.Count * _textBox.FontSizeManager.GetFontHeight();
        return render;
    }

    public void Dispose()
    {
        _textBox = null;
    }
}