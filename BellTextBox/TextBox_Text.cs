﻿using Bell.Data;

namespace Bell;

public partial class TextBox
{
    public List<Line> Lines = new();
    
    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;
    
    public List<LineRender> ShowLineRenders => ShowLineRendersCache.Get();
    public readonly Cache<List<LineRender>> ShowLineRendersCache;
    
    public void SetText(string text)
    {
        Lines.Clear();
        int i = 0;
        foreach (string lineText in text.Split("\n"))
        {
            Line line = new Line(this, i++);
            line.SetString(lineText.Trim('\r'));
            Lines.Add(line);
        }
        
        LineRendersCache.SetDirty();
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
    
    private List<LineRender> UpdateShowLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        var pageStart = ViewToPage(ViewStart);
        var pageEnd = ViewToPage(ViewEnd);

        var textStart = PageToText(pageStart, -3);
        var textEnd = PageToText(pageEnd, 3);

        for (int i = textStart.Row; i <= textEnd.Row; i++)
        {
            if (LineRenders.Count > i)
                lineRenders.Add(LineRenders[i]);
        }

        return lineRenders;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}