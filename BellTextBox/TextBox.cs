using System.Numerics;
using System.Text;
using Bell.Commands;
using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public partial class TextBox
{
    // Data
    public Page Page { get; private set; }
    public Text Text { get; private set; }
    
    
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();
    public FontSizeManager FontSizeManager { get; private set; }
    public CoordinatesManager CoordinatesManager { get; private set; }

    // Action
    internal readonly CommandSetHistory CommandSetHistory = new();
    internal readonly Cursor Cursor = new();

    public TextBox()
    {
        Page = new Page(this);
        Text = new Text(this);
        
        FontSizeManager = new FontSizeManager(this);
        CoordinatesManager = new CoordinatesManager(this);

        KeyboardInput.Chars = new List<char>();
    }

    // Method
    public List<int> FindText(string text)
    {
        return new();
    }

    public bool Goto(int lineNumber)
    {
        return true;
    }

    public bool Fold(int lineNumber)
    {
        return true;
    }

    public bool Unfold(int lineNumber)
    {
        return true;
    }

    public void SetText(string text)
    {
        Text.SetText(text);
        Page.RenderCache.SetDirty();
    }

    private void DoAction(Command command)
    {
        CommandSet commandSet = new CommandSet();
        commandSet.Add(command);
        DoActionSet(commandSet);
    }

    private void DoActionSet(CommandSet commandSet)
    {
        commandSet.Do(this);
        CommandSetHistory.AddHistory(commandSet);
    }
}