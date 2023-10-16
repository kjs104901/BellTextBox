using System.Text;
using Bell.Data;

namespace Bell;

public partial class TextBox
{
    public Text Text { get; private set; }
    
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();

    public readonly Theme Theme;

    protected TextBox()
    {
        Text = new Text(this);

        KeyboardInput.Chars = new List<char>();

        Theme = new DarkTheme();
    }

    public void SetText(string text)
    {
        Text.SetText(text);
    }
}