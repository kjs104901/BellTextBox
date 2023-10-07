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

public partial class TextBox
{
    public bool AutoIndent { get; set; } = true;
    public bool AutoComplete { get; set; } = true;
    public bool Overwrite { get; set; } = false;
    public bool ReadOnly { get; set; } = false;
    public WrapMode WrapMode { get; set; } = WrapMode.Word;
    public EolMode EolMode = EolMode.LF;
    public bool SyntaxHighlighting { get; set; } = true;
    public bool SyntaxFolding { get; set; } = true;
    public Language Language { get; set; } = Language.PlainText();
    
    public float LineNumberWidth { get; set; } = 20.0f;
    public float MarkerWidth { get; set; } = 10.0f;
}