using Bell.Render;

namespace Bell.Data;

public class Text
{
    private readonly TextBox _textBox;
    
    private readonly List<Line> _lines = new();
    public List<LineView> LineViews => _lineViewCache.Get();
    private readonly Cache<List<LineView>> _lineViewCache;

    public List<LineRender> LineRenders = new(); //TODO cache?
    
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
        LineRenders.Clear();
        foreach (LineView lineView in LineViews)
        {
            LineRenders.Add(_lines[lineView.LineIndex].LineRender);
        }
        return LineRenders;
    }

    private List<LineView> UpdateLineViews(List<LineView> lineViews)
    {
        lineViews.Clear();
        int i = 0;
        foreach (Line line in _lines)
        {
            lineViews.Add(new LineView { LineIndex = i, WrapIndex = 0 });
            i++;
        }
        return lineViews;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}