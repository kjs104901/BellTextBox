using System.Text;
using Bell.Data;

namespace Bell;

public partial class TextBox
{
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();

    public readonly Theme Theme;

    protected TextBox()
    {
        Theme = new DarkTheme();
        
        SubLinesCache = new Cache<List<SubLine>>(new List<SubLine>(), UpdateSubLines);
        VisibleSubLinesCache = new Cache<List<SubLine>>(new List<SubLine>(), UpdateVisibleSubLines);
        FoldingListCache = new Cache<List<Folding>>(new List<Folding>(), UpdateFoldingList);
    }
}