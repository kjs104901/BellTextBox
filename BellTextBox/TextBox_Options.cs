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
    public EolMode EolMode = EolMode.LF;
    public TabMode TabMode = TabMode.Space;
    public int TabSize = 4;
    public bool SyntaxHighlighting { get; set; } = true;
    public bool SyntaxFolding { get; set; } = true;
    
    public float LeadingHeight { get; set; } = 1.2f;
    public float LetterSpacing { get; set; } = 0.0f;
    
    public Language Language { get; set; } = Language.PlainText();
}