using System.Numerics;
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

    public Page(TextBox textBox)
    {
        _textBox = textBox;

        RenderCache = new Cache<PageRender>(new PageRender(), UpdateRender);
        LineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateLineRenders);
    }


    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        var pageStart = _textBox.CoordinatesManager.ViewToPage(_textBox.ViewStart);
        var pageEnd = _textBox.CoordinatesManager.ViewToPage(_textBox.ViewEnd);

        var textStart = _textBox.CoordinatesManager.PageToText(pageStart, -3);
        var textEnd = _textBox.CoordinatesManager.PageToText(pageEnd, 3);

        for (int i = textStart.Row; i <= textEnd.Row; i++)
        {
            LineWrap lineWrap = _textBox.Text.LineWraps[i];
            var lineRender = _textBox.Text.Lines[lineWrap.LineIndex].GetLineRender(lineWrap.RenderIndex);

            lineRender.PosX = 0;
            lineRender.PosY = i * _textBox.FontSizeManager.GetFontSize();

            lineRenders.Add(lineRender);
        }

        return lineRenders;
    }

    private PageRender UpdateRender(PageRender render)
    {
        if (WrapMode.None == _textBox.WrapMode)
        {
            render.Size.X = 500; // TODO find max render width
        }
        else
        {
            render.Size.X = _textBox.PageWidth;
        }
        
        render.Size.Y = _textBox.Text.LineWraps.Count * _textBox.FontSizeManager.GetFontSize();
        return render;
    }
}