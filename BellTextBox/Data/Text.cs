using Bell.Render;

namespace Bell.Data;

public class Text
{
    private readonly TextBox _textBox;
    
    private readonly List<Line> _lines = new();
    public List<LineView> LineViews => _lineViewCache.Get();
    private readonly Cache<List<LineView>> _lineViewCache;

    private readonly List<LineRender> _lineRenders = new();
    
    public Text(TextBox textBox)
    {
        _textBox = textBox;

        _lineViewCache = new Cache<List<LineView>>(new List<LineView>(), UpdateLineViews);
    }
    
    public void Set(string text)
    {
        _lines.Clear();
        foreach (string lineText in text.Split("\n"))
        {
            Line line = new Line(_textBox);
            line.SetString(lineText);
            _lines.Add(line);
        }
        _lineViewCache.SetDirty();
    }
    
    public List<LineRender> GetLineRenders()
    {
        _lineRenders.Clear();
        foreach (LineView lineView in LineViews)
        {
            _lineRenders.Add(_lines[lineView.LineIndex].GetLineRender(lineView.RenderIndex));
        }
        return _lineRenders;
    }

    private List<LineView> UpdateLineViews(List<LineView> lineViews)
    {
        lineViews.Clear();
        int i = 0;
        foreach (Line line in _lines)
        {
            for (int j = 0; j < line.RenderCount; j++)
            {
                lineViews.Add(new LineView { LineIndex = i, RenderIndex = j });
            }
            i++;
        }
        return lineViews;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}