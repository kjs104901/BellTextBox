using Bell.Coordinates;
using Bell.Render;

namespace Bell.Data;

public class Page
{
    private TextBox _textBox;

    public PageRender Render => RenderCache.Get();
    public readonly Cache<PageRender> RenderCache;

    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;

    // View
    private ViewCoordinates _viewStart;
    private ViewCoordinates _viewEnd;

    public Page(TextBox textBox)
    {
        _textBox = textBox;

        RenderCache = new Cache<PageRender>(new PageRender(), UpdateRender);
        LineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateLineRenders);
    }

    public void UpdateView(ViewCoordinates start, ViewCoordinates end)
    {
        _viewStart = start;
        _viewEnd = end;

        LineRendersCache.SetDirty();
    }

    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        var pageStart = _textBox.CoordinatesManager.ViewToPage(_viewStart);
        var pageEnd = _textBox.CoordinatesManager.ViewToPage(_viewEnd);

        var textStart = _textBox.CoordinatesManager.PageToText(pageStart, -3);
        var textEnd = _textBox.CoordinatesManager.PageToText(pageEnd, 3);

        for (int i = textStart.Row; i <= textEnd.Row; i++)
        {
            LineWrap lineWrap = _textBox.Text.LineWraps[i];
            var lineRender = _textBox.Text.Lines[lineWrap.LineIndex].GetLineRender(lineWrap.RenderIndex);

            lineRender.PosX = 0;
            lineRender.PosY = i * _textBox.FontSizeManager.GetFontHeight();

            lineRenders.Add(lineRender);
        }

        return lineRenders;
    }

    private PageRender UpdateRender(PageRender render)
    {
        render.Size.X = 500; //TODO find width from render
        render.Size.Y = _textBox.Text.LineWraps.Count * _textBox.FontSizeManager.GetFontHeight();
        return render;
    }
}