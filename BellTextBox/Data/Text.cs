using Bell.Render;

namespace Bell.Data;

public class Text
{
    private TextBox _textBox;

    private string _textString = "";
    
    public List<Line> Lines => LinesCache.Get();
    public readonly Cache<List<Line>> LinesCache;
    
    public List<LineWrap> LineWraps => LineWrapsCache.Get();
    public readonly Cache<List<LineWrap>> LineWrapsCache;
    
    public Text(TextBox textBox)
    {
        _textBox = textBox;

        LinesCache = new Cache<List<Line>>(new List<Line>(), UpdateLines);
        LineWrapsCache = new Cache<List<LineWrap>>(new List<LineWrap>(), UpdateLineWraps);
    }
    
    public void SetText(string text)
    {
        _textString = text;
        
        LinesCache.SetDirty();
        LineWrapsCache.SetDirty();
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
    
    private List<LineWrap> UpdateLineWraps(List<LineWrap> lineViews)
    {
        lineViews.Clear();
        int i = 0;
        foreach (Line line in Lines)
        {
            for (int j = 0; j < line.RenderCount; j++)
            {
                lineViews.Add(new LineWrap { LineIndex = i, RenderIndex = j });
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