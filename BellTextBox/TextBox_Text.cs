using Bell.Data;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    public List<Line> Lines = new();
    
    public List<SubLine> Rows => RowsCache.Get();
    public readonly Cache<List<SubLine>> RowsCache;
    
    public void SetText(string text)
    {
        Lines.Clear();
        int i = 0;
        foreach (string lineText in text.Split("\n"))
        {
            Line line = new Line(i++);
            line.SetString(lineText.Trim('\r'));
            Lines.Add(line);
        }
        
        RowsCache.SetDirty();
    }
    
    private List<SubLine> UpdateRows(List<SubLine> rows)
    {
        rows.Clear();

        int row = 0;
        int foldingCount = 0;
        foreach (Line line in Lines)
        {
            bool visible = true;
            
            Folding? lineFolding = null;
            
            foreach (Folding folding in FoldingList)
            {
                if (folding.End == line.Index)
                {
                    foldingCount--;
                }
                
                if (folding.Start < line.Index && line.Index < folding.End)
                {
                    if (folding.Folded)
                    {
                        visible = (0 == foldingCount);
                        break;
                    }
                }
                
                if (folding.Start == line.Index)
                {
                    lineFolding = folding;
                    foldingCount++;
                }

            }
            
            if (visible)
            {
                foreach (SubLine subLine in line.SubLines)
                {
                    subLine.Row = row++;
                    subLine.Folding = lineFolding;
                    rows.Add(subLine);
                }
            }
        }
        return rows;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}