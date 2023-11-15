using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public static Language Json()
    {
        Language language = new()
        {
            DefaultStyle = new ColorStyle(0.4f, 0.8f, 0.2f),
            CommentStyle = new ColorStyle(0.3f, 0.5f, 0.2f),
            StringStyle = new ColorStyle(0.2f, 0.3f, 0.3f)
        };

        language.AddFolding("{", "}");
        language.AddFolding("[", "]");
        
        // Key
        language.AddPattern(@"(?<range>""([^\\""]|\\"")*"")\s*:",
            new ColorStyle(0.5f, 0.4f, 0.8f));
        
        // Number
        language.AddPattern(@"-?\b\d+(\.\d+)?([eE][+-]?\d+)?\b",
            new ColorStyle(0.8f, 0.4f, 0.2f));
        
        // Keyword
        language.AddPattern(@"\b(true|false|null)\b",
            new ColorStyle(0.4f, 0.8f, 0.8f));

        return language;
    }
}