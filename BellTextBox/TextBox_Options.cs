using Bell.Languages;

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

    public int CountTabStart(string line)
    {
        string tabString = TabMode.Space == TabMode ? new string(' ', TabSize) : "\t";

        int count = 0;
        while (line.StartsWith(tabString))
        {
            count++;
            line = line.Remove(0, tabString.Length);
        }
        return count;
    }

    public float GetTabRenderSize()
    {
        if (WordWrapIndent)
            return GetCharWidth(' ') * TabSize;
        return 0.0f;
    }
}