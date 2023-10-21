using System.Text;
using Bell.Data;
using Bell.Utils;
using Action = Bell.Data.Action;

namespace Bell;

public partial class TextBox
{
    public readonly List<string> AutoCompleteList = new();

    public readonly Theme Theme;
    private readonly IBackend _backend;
    
    public TextBox(IBackend backend)
    {
        _backend = backend;

        Theme = new DarkTheme();
        
        RowsCache = new Cache<List<SubLine>>(new List<SubLine>(), UpdateRows);
        FoldingListCache = new Cache<List<Folding>>(new List<Folding>(), UpdateFoldingList);
    }

    public string GetDebugString()
    {
        var sb = new StringBuilder();
        foreach (Action action in _actionHistory)
        {
            sb.AppendLine(action.GetDebugString());
        }
        return sb.ToString();
    }
}