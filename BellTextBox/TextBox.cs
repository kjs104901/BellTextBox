using System.Text;
using Bell.Actions;
using Bell.Data;
using Bell.Themes;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    internal readonly IBackend Backend;

    internal readonly ActionManager ActionManager = new();
    internal readonly CaretManager CaretManager = new();
    internal readonly FontManager FontManager = new();
    internal readonly LineManager LineManager = new();
    internal readonly RowManager RowManager = new();
    internal readonly FoldingManager FoldingManager = new();
    internal readonly Logger Logger = new ();
    internal readonly CacheCounter CacheCounter = new();
    
    private readonly StringBuilder _sb = new();

    public TextBox(IBackend backend)
    {
        Backend = backend;

        Theme = new DarkTheme();
    }

    public string GetDebugString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(CaretManager.GetDebugString());
        sb.AppendLine(CacheCounter.GetDebugString());
        sb.AppendLine(ActionManager.GetDebugString());
        return sb.ToString();
    }

    public List<string> GetLogs()
    {
        return Logger.GetLogs().Select(i => $"[{i.Item1}] {i.Item3}: ({i.Item2})").ToList();
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
        CaretManager.ClearCarets();
        RowManager.SetRowCacheDirty();
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