using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class LineManager
{
    public List<Line> Lines = new();
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

    public Line InsertLine(int lineIndex, char[] lineChars)
    {
        Line newLine = new Line(lineIndex, lineChars);
        Lines.Insert(lineIndex, newLine);

        // Update line index
        for (int i = lineIndex; i < Lines.Count; i++)
        {
            Lines[i].Index = i;
        }

        Singleton.RowManager.RowsCache.SetDirty();
        Singleton.FoldingManager.FoldingListCache.SetDirty();

        return newLine;
    }

    public void RemoveLine(int removeLineIndex)
    {
        Lines.RemoveAt(removeLineIndex);

        // Update line index
        for (int i = removeLineIndex; i < Lines.Count; i++)
        {
            Lines[i].Index = i;
        }

        Singleton.RowManager.RowsCache.SetDirty();
        Singleton.FoldingManager.FoldingListCache.SetDirty();
    }
}