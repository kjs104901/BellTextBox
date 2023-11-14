using Bell.Utils;

namespace Bell.Data;

// Interface
internal partial class RowManager
{
    internal static List<Row> Rows => Singleton.TextBox.RowManager._rows;
    internal static void SetRowCacheDirty() => Singleton.TextBox.RowManager.SetRowCacheDirty_();
}

// Implementation
internal partial class RowManager
{
    private List<Row> _rows => _rowsCache.Get();
    private readonly Cache<List<Row>> _rowsCache;

    internal RowManager()
    {
        _rowsCache = new Cache<List<Row>>("Rows", new List<Row>(), UpdateRows);
    }

    private void SetRowCacheDirty_()
    {
        _rowsCache.SetDirty();
        foreach (Row row in Rows)
        {
            row.RowSelectionCache.SetDirty();
        }
    }

    private List<Row> UpdateRows(List<Row> rows)
    {
        rows.Clear();

        int foldingCount = 0;
        foreach (Line line in LineManager.Lines)
        {
            bool visible = true;

            line.Folding = Folding.None;
            foreach (Folding folding in LineManager.FoldingList)
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
                foreach (var lineSub in line.LineSubs)
                {
                    rows.Add(new Row(lineSub));
                }
            }
        }

        return rows;
    }
}