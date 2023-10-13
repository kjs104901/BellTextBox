using System.Numerics;
using Bell.Coordinates;

namespace Bell.Data;

public class Page
{
    private TextBox _textBox;

    public Vector2 Size => SizeCache.Get();
    public readonly Cache<Vector2> SizeCache;

    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;

    public Page(TextBox textBox)
    {
        _textBox = textBox;

        SizeCache = new Cache<Vector2>(new Vector2(), UpdateSize);
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
            if (_textBox.Text.LineRenders.Count > i)
                lineRenders.Add(_textBox.Text.LineRenders[i]);
        }

        return lineRenders;
    }

    private Vector2 UpdateSize(Vector2 size)
    {
        if (WrapMode.None == _textBox.WrapMode)
        {
            size.X = 500; // TODO find max render width
        }
        else
        {
            size.X = _textBox.PageWidth;
        }
        
        size.Y = _textBox.Text.LineRenders.Count * _textBox.FontSizeManager.GetFontSize();
        return size;
    }
}