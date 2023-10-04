using Bell.Render;

namespace Bell.Data;

public class Text
{
    private readonly TextBox _textBox;
    
    private readonly List<Line> _lines = new();

    public List<LineRender> LineRenders => _lineRendersCache.Get();
    private readonly Cache<List<LineRender>> _lineRendersCache;
    
    public Text(TextBox textBox)
    {
        _textBox = textBox;

        _lineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateRender);
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
        _lineRendersCache.SetDirty();
    }

    private List<LineRender> UpdateRender(List<LineRender> lineRenders)
    {
        lineRenders.Clear();
        foreach (Line line in _lines)
        {
            lineRenders.Add(line.GetRender());
        }
        return lineRenders;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}