using Bell.Data;
using Bell.Languages;
using Bell.Themes;

namespace Bell;

public enum WrapMode
{
    None,
    Word,
    BreakWord
}

public enum EolMode
{
    CRLF,
    LF,
    CR
}

public enum TabMode
{
    Space,
    Tab
}

public partial class TextBox
{
    public readonly Theme Theme;
    
    public bool AutoIndent { get; set; } = true;
    public bool AutoComplete { get; set; } = true;
    public bool Overwrite { get; set; } = false;
    public bool ReadOnly { get; set; } = false;
    public WrapMode WrapMode { get; set; } = WrapMode.Word;
    public bool WordWrapIndent { get; set; } = true;
    public TabMode TabMode = TabMode.Space;
    public int TabSize = 4;
    
    public EolMode EolMode = EolMode.LF;
    
    public bool SyntaxHighlighting { get; set; } = true;
    public bool SyntaxFolding { get; set; } = true;

    public bool SyntaxHighlightEnabled { get; set; } = true;
    public bool ShowingWhitespace { get; set; } = true;
    
    public float LeadingHeight { get; set; } = 1.2f;
    
    public Language Language { get; set; } = Language.PlainText();
    
    public readonly List<string> AutoCompleteList = new();

    internal int CountTabStart(string line)
    {
        string tabString = GetTabString();

        int count = 0;
        while (line.StartsWith(tabString))
        {
            count++;
            line = line.Remove(0, tabString.Length);
        }
        return count;
    }

    internal float GetTabRenderSize()
    {
        return FontManager.GetFontWhiteSpaceWidth() * TabSize;
    }

    internal string GetTabString()
    {
        if (TabMode.Space == TabMode)
            return new string(' ', TabSize);
        return "\t";
    }

    private string ReplaceTab(string text)
    {
        switch (TabMode)
        {
            case TabMode.Space:
                return text.Replace("\t", new string(' ', TabSize));
            case TabMode.Tab:
                return text.Replace(new string(' ', TabSize), "\t");
        }
        return text;
    }

    private string ReplaceEol(string text)
    {
        return text.Replace("\r\n", "\n")
            .Replace("\r", "\n");
    }

    private string GetEolString()
    {
        switch (EolMode)
        {
            case EolMode.CRLF:
                return "\r\n";
            case EolMode.LF:
                return "\n";
            case EolMode.CR:
                return "\r";
        }
        return "\n";
    }
}