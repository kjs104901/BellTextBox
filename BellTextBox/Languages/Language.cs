using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public List<string> LineComments = new();
    public Style LineCommentStyle = new(0.0f, 1.0f, 0.0f, 1.0f);
    
    public List<Block> BlockComments = new();
    public Style BlockCommentStyle = new(0.2f, 1.0f, 0.0f, 1.0f);

    public List<Block> Foldings = new();

    public Dictionary<string, Style> PatternsStyle = new();
    public Dictionary<string, Style> KeywordsStyle = new();
    
    public string AutoIndentPattern = "";
}