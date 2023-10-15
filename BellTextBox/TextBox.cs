using System.Numerics;
using System.Text;
using Bell.Actions;
using Bell.Carets;
using Bell.Colors;
using Bell.Coordinates;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;
using Action = Bell.Actions.Action;

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
    internal ActionManager ActionManager { get; }

    public readonly Theme Theme;

    public TextBox()
    {
        Page = new Page(this);
        Text = new Text(this);
        
        FontSizeManager = new FontSizeManager(this);
        CoordinatesManager = new CoordinatesManager(this);
        CaretManager = new CaretManager(this);
        ActionManager = new ActionManager(this);

        KeyboardInput.Chars = new List<char>();

        Theme = new DarkTheme();
    }

    public void SetText(string text)
    {
        Text.SetText(text);
        Page.SizeCache.SetDirty();
    }
}