using System.Numerics;
using System.Text;
using Bell.Carets;
using Bell.Commands;
using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Bell.Render;
using Action = Bell.Commands.Action;

namespace Bell;

public partial class TextBox
{
    // Data
    public Page Page { get; private set; }
    public Text Text { get; private set; }
    
    
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();
    public FontSizeManager FontSizeManager { get; }
    public CoordinatesManager CoordinatesManager { get; }
    internal CaretManager CaretManager { get; }

    // Action
    internal readonly ActionHistory ActionHistory = new();

    public readonly Theme Theme;

    public TextBox()
    {
        Page = new Page(this);
        Text = new Text(this);
        
        FontSizeManager = new FontSizeManager(this);
        CoordinatesManager = new CoordinatesManager(this);
        CaretManager = new CaretManager(this);

        KeyboardInput.Chars = new List<char>();

        Theme = new DarkTheme();
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
        Action action = new Action();
        action.Add(command);
        DoActionSet(action);
    }

    private void DoActionSet(Action action)
    {
        action.Do(this);
        ActionHistory.AddHistory(action);
    }
}