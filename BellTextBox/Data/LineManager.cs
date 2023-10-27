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
            if (line.Index != lineIndex)
            {
                Logger.Error($"LineManager Line.Index != lineIndex: {line.Index} != {lineIndex}");
            }
            return true;
        }

        line = Line.None;
        return false;
    }

    public Line InsertLine(int lineIndex, char[] lineChars)
    {
        Line newLine = new Line(lineIndex, lineChars);
        Lines.Insert(lineIndex, newLine);

        // Update line index
        for (int i = lineIndex; i < Lines.Count; i++)
        {
            if (Lines[i].Index != i)
            {
                Lines[i].Index = i;
                //foreach (LineSub lineSub in Lines[i].LineSubs)
                //{
                //    lineSub.Coordinates.LineIndex = i;
                //}
                Lines[i].LineSubsCache.SetDirty();
            }
        }
        Singleton.RowManager.RowsCache.SetDirty();
        return newLine;
    }

    public void RemoveLine(int removeLineIndex)
    {
        Lines.RemoveAt(removeLineIndex);

        // Update line index
        for (int i = removeLineIndex; i < Lines.Count; i++)
        {
            if (Lines[i].Index != i)
            {
                Lines[i].Index = i;
                //foreach (LineSub lineSub in Lines[i].LineSubs)
                //{
                //    lineSub.Coordinates.LineIndex = i;
                //}
                Lines[i].LineSubsCache.SetDirty();
            }
        }
        Singleton.RowManager.RowsCache.SetDirty();
    }
}