namespace Bell.Data;

public class Text
{
    private TextBox _textBox;

    private string _textString = "";
    
    public List<Line> Lines => LinesCache.Get();
    public readonly Cache<List<Line>> LinesCache;
    
    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;
    
    public Text(TextBox textBox)
    {
        _textBox = textBox;

        LinesCache = new Cache<List<Line>>(new List<Line>(), UpdateLines);
        LineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateLineRenders);
    }
    
    public void SetText(string text)
    {
        _textString = text;
        
        LinesCache.SetDirty();
        LineRendersCache.SetDirty();
    }

    private List<Line> UpdateLines(List<Line> lines)
    {
        lines.Clear();

        int i = 0;
        foreach (string lineText in _textString.Split("\n"))
        {
            Line line = new Line(_textBox, i++);
            line.SetString(lineText.Trim('\r'));
            lines.Add(line);
        }
        return lines;
    }
    
    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        int row = 0;
        foreach (Line line in Lines)
        {
            if (line.Visible)
            {
                foreach (LineRender lineRender in line.LineRenders)
                {
                    lineRender.Row = row++;
                    lineRenders.Add(lineRender);
                }
            }
        }
        return lineRenders;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}