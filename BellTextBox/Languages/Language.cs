using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public List<string> LineComments = new();
    public FontStyle LineCommentFontStyle = FontStyle.LineCommentFontStyle;
    
    public List<Block> BlockComments = new();
    public FontStyle BlockCommentFontStyle = FontStyle.BlockCommentFontStyle;

    public List<Block> Foldings = new();

    public Dictionary<string, FontStyle> PatternsStyle = new();
    public Dictionary<string, FontStyle> KeywordsStyle = new();
    
    public string AutoIndentPattern = "";
}