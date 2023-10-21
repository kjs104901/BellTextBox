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
        string tabString = GetTabString();

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
        return GetFontWhiteSpaceWidth() * TabSize;
    }

    public string ReplaceTab(string text)
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
    
    public string GetTabString()
    {
        if (TabMode.Space == TabMode)
            return new string(' ', TabSize);
        return "\t";
    }

    public string ReplaceEol(string text)
    {
        return text.Replace("\r\n", "\n")
            .Replace("\r", "\n");
    }
    
    public string GetEolString()
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