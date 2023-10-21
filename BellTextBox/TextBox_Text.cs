using System.Text;
using Bell.Data;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    public List<Line> Lines = new();

    public List<SubLine> Rows => RowsCache.Get();
    public readonly Cache<List<SubLine>> RowsCache;
    
    private readonly StringBuilder _sb = new();

    public void SetText(string text)
    {
        text = ReplaceTab(text);
        text = ReplaceEol(text);

        Lines.Clear();
        int i = 0;
        foreach (string lineText in text.Split('\n'))
        {
            Line line = new Line(i++);
            line.SetString(lineText.Trim('\r'));
            Lines.Add(line);
        }

        RowsCache.SetDirty();
    }

    public string GetText()
    {
        _sb.Clear();
        foreach (Line line in Lines)
        {
            _sb.Append(line.String);
            _sb.Append(GetEolString());
        }

        return _sb.ToString();
    }

    private List<SubLine> UpdateRows(List<SubLine> rows)
    {
        rows.Clear();

        int row = 0;
        int foldingCount = 0;
        foreach (Line line in Lines)
        {
            bool visible = true;

            line.Folding = null;
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

    public override string ToString()
    {
        return string.Empty;
    }
}