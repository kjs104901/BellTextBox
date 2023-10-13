using Bell.Colors;
using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public List<string> LineComments = new();
    public List<ValueTuple<string, string>> BlockComments = new();

    public List<ValueTuple<string, string>> Foldings = new();

    public Dictionary<string, ColorStyle> PatternsStyle = new();
    public Dictionary<string, ColorStyle> KeywordsStyle = new();
    
    public string AutoIndentPattern = "";
}