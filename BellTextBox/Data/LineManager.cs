using System.Text;
using Bell.Utils;

namespace Bell.Data;

public class LineManager
{
    public List<Line> Lines = new();

    public List<SubLine> Rows => RowsCache.Get();
    public readonly Cache<List<SubLine>> RowsCache;
    
    public LineManager()
    {
        RowsCache = new Cache<List<SubLine>>(new List<SubLine>(), UpdateRows);
    }

    private List<SubLine> UpdateRows(List<SubLine> rows)
    {
        rows.Clear();

        int row = 0;
        int foldingCount = 0;
        foreach (Line line in Lines)
        {
            bool visible = true;

            line.Folding = Folding.None;
            foreach (Folding folding in ThreadLocal.FoldingManager.FoldingList)
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
                    line.Folding = folding;
                    foldingCount++;
                }
            }

            if (visible)
            {
                foreach (SubLine subLine in line.SubLines)
                {
                    subLine.Row = row++;
                    rows.Add(subLine);
                }
            }
        }

        return rows;
    }
    
    public bool GetLine(int lineIndex, out Line line)
    {
        if (0 <= lineIndex && lineIndex < Lines.Count)
        {
            line = Lines[lineIndex];
            return true;
        }

        line = Line.Empty;
        return false;
    }

    public void InsertLine(int insertLineIndex, char[] lineChars)
    {
        Line newLine = new Line(insertLineIndex, lineChars);
        Lines.Insert(insertLineIndex, newLine);
        
        // Update line index
        int i = 0;
        foreach (Line textBoxLine in Lines)
        {
            textBoxLine.Index = i++;
            textBoxLine.SetCharsDirty();
        }
        
        RowsCache.SetDirty();
        ThreadLocal.FoldingManager.FoldingListCache.SetDirty();
    }

    public void RemoveLine(int removeLineIndex)
    {
        Lines.RemoveAt(removeLineIndex);
        
        // Update line index
        int i = 0;
        foreach (Line textBoxLine in Lines)
        {
            textBoxLine.Index = i++;
            textBoxLine.SetCharsDirty();
        }
        
        RowsCache.SetDirty();
        ThreadLocal.FoldingManager.FoldingListCache.SetDirty();
    }
}