using Bell.Render;

namespace Bell.Data;

public class Text
{
    private readonly TextBox _textBox;

    private string _textString = "";
    
    public List<Line> Lines => _linesCache.Get();
    private readonly Cache<List<Line>> _linesCache;
    
    public List<LineView> LineViews => _lineViewCache.Get();
    private readonly Cache<List<LineView>> _lineViewCache;
    
    public Text(TextBox textBox)
    {
        _textBox = textBox;

        _linesCache = new Cache<List<Line>>(new List<Line>(), UpdateLines);
        _lineViewCache = new Cache<List<LineView>>(new List<LineView>(), UpdateLineViews);
    }
    
    public void SetText(string text)
    {
        _textString = text;
        
        _linesCache.SetDirty();
        _lineViewCache.SetDirty();
    }

    private List<Line> UpdateLines(List<Line> lines)
    {
        lines.Clear();
        foreach (string lineText in _textString.Split("\n"))
        {
            Line line = new Line(_textBox);
            line.SetString(lineText.Trim('\r'));
            lines.Add(line);
        }
        return lines;
    }
    
    private List<LineView> UpdateLineViews(List<LineView> lineViews)
    {
        lineViews.Clear();
        int i = 0;
        foreach (Line line in Lines)
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