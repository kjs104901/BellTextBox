using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public List<string> LineComments = new();
    public List<Block> BlockComments = new();

    public List<Block> Foldings = new();

    public Dictionary<string, Style> PatternsStyle = new();
    public Dictionary<string, Style> KeywordsStyle = new();
    
    public string AutoIndentPattern = "";
}