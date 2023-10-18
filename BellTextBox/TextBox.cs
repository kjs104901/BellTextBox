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
        
        LineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateLineRenders);
        ShowLineRendersCache = new Cache<List<LineRender>>(new List<LineRender>(), UpdateShowLineRenders);
        FoldingListCache = new Cache<List<Folding>>(new List<Folding>(), UpdateFoldingList);
    }
}