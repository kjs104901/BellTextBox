using System.Text;
using Bell.Actions;
using Bell.Data;
using Bell.Themes;
using Bell.Utils;
using Action = Bell.Actions.Action;

namespace Bell;

public partial class TextBox
{
    public readonly List<string> AutoCompleteList = new();

    public readonly Theme Theme;
    public readonly IBackend _backend;
    
    public readonly ActionManager ActionManager = new();
    public readonly CaretManager CaretManager = new();
    public readonly FontManager FontManager = new();
    public readonly LineManager LineManager = new();
    public readonly FoldingManager FoldingManager = new();
    public readonly Logger Logger = new ();
    
    private readonly StringBuilder _sb = new();

    public TextBox(IBackend backend)
    {
        _backend = backend;

        Theme = new DarkTheme();
    }

    public string GetDebugString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(ActionManager.GetDebugString());
        return sb.ToString();
    }

    public List<string> GetLogs()
    {
        return Logger.GetLogs().Select(i => $"[{i.Item1}] ({i.Item2}) {i.Item3}").ToList();
    }
    
    public void SetText(string text)
    {
        Singleton.TextBox = this;
        
        text = Singleton.TextBox.ReplaceTab(text);
        text = Singleton.TextBox.ReplaceEol(text);

        LineManager.Lines.Clear();
        int i = 0;
        foreach (string lineText in text.Split('\n'))
        {
            Line line = new Line(i++, lineText.ToArray());
            LineManager.Lines.Add(line);
        }

        LineManager.RowsCache.SetDirty();
    }

    public string GetText()
    {
        Singleton.TextBox = this;
        
        _sb.Clear();
        foreach (Line line in LineManager.Lines)
        {
            _sb.Append(line.String);
            _sb.Append(Singleton.TextBox.GetEolString());
        }
        return _sb.ToString();
    }

}