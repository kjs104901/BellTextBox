using System.Text;
using Bell.Data;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();

    public readonly Theme Theme;
    
    private readonly IBackend _backend;
    
    private static readonly ThreadLocal<TextBox> ThreadLocalTextBox = new();
    public static TextBox Get() => ThreadLocalTextBox.Value ?? throw new Exception("No TextBox set");
    private static void Set(TextBox textBox) => ThreadLocalTextBox.Value = textBox;

    public TextBox(IBackend backend)
    {
        _backend = backend;

        Theme = new DarkTheme();
        
        SubLinesCache = new Cache<List<SubLine>>(new List<SubLine>(), UpdateSubLines);
        FoldingListCache = new Cache<List<Folding>>(new List<Folding>(), UpdateFoldingList);
    }
}