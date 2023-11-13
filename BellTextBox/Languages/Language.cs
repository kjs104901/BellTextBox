using Bell.Data;
using Bell.Themes;

namespace Bell.Languages;

public partial class Language
{
    public List<string> LineComments = new();
    public List<ValueTuple<string, string>> BlockComments = new();
    public List<ValueTuple<string, string>> Texts = new();
    public List<ValueTuple<string, string>> Foldings = new();

    public Dictionary<string, ColorStyle> PatternsStyle = new();
    public Dictionary<string, ColorStyle> KeywordsStyle = new();
    
    public string AutoIndentPattern = "";

    internal enum TokenType
    {
        LineComment,
        BlockCommentStart,
        BlockCommentEnd,
        TextStart,
        TextEnd,
        FoldingStart,
        FoldingEnd
    }
    
    internal struct Token
    {
        internal TokenType Type;
        internal string String;
    }
    
    internal struct Syntax
    {
        internal Token Token;
        internal int CharIndex;
    }
}