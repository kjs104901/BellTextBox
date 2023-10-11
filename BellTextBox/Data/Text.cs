using Bell.Render;

namespace Bell.Data;

public class Text : IDisposable
{
    private TextBox? _textBox;

    private string _textString = "";
    
    public List<Line> Lines => LinesCache.Get();
    public readonly Cache<List<Line>> LinesCache;
    
    public List<LineView> LineViews => LineViewCache.Get();
    public readonly Cache<List<LineView>> LineViewCache;
    
    public Text(TextBox? textBox)
    {
        _textBox = textBox;

        LinesCache = new Cache<List<Line>>(new List<Line>(), UpdateLines);
        LineViewCache = new Cache<List<LineView>>(new List<LineView>(), UpdateLineViews);
    }
    
    public void SetText(string text)
    {
        _textString = text;
        
        LinesCache.SetDirty();
        LineViewCache.SetDirty();
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

    public void Dispose()
    {
        foreach (Line line in Lines)
        {
            line.Dispose();
        }
        Lines.Clear();
        
        _textBox = null;
    }
}