using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public static Language CSharp()
    {
        Language language = new()
        {
            DefaultStyle = new ColorStyle(0.4f, 0.8f, 0.2f),
            CommentStyle = new ColorStyle(0.3f, 0.5f, 0.2f),
            StringStyle = new ColorStyle(0.2f, 0.3f, 0.3f)
        };

        language.AddFolding("{", "}");
        language.AddFolding("#region", "#endregion");
        language.AddFolding("#if", "#endif");

        language.AddLineComment("//");

        language.AddBlockComment("/*", "*/");

        language.AddString("'");
        language.AddString("\"");
        language.AddString("\"\"\"\"");

        language.AddMultilineString("@\"", "\"");
        language.AddMultilineString("$@\"", "\"");
        language.AddMultilineString("@$\"", "\"");

        // Number
        language.AddPattern(@"(\b0x[0-9a-fA-F]+\b|\b0b[01]+\b|\b0[0-7]+\b|\b\d+(\.\d+)?([eE][-+]?\d+)?[lLdDfF]?\b)",
            new ColorStyle(0.5f, 0.4f, 0.8f));
        
        // Attribute
        language.AddPattern(@"\[\s*[a-zA-Z0-9_\.]+\s*(\([^\)]*\))?\s*\]",
            new ColorStyle(0.8f, 0.4f, 0.2f));
        
        // Class
        language.AddPattern(@"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b",
            new ColorStyle(0.8f, 0.8f, 0.2f));
        
        // Keyword
        language.AddPattern(@"\b(public|private|protected|internal|class|struct|interface|enum|delegate|int|long|float|double|bool|char|string|object|byte|sbyte|short|ushort|uint|ulong|decimal|if|else|switch|case|do|for|foreach|while|break|continue|return|goto|throw|try|catch|finally|checked|unchecked|fixed|lock|new|override|virtual|abstract|sealed|static|readonly|extern|ref|out|in|params|using|namespace|true|false|null|this|base|operator|sizeof|typeof|stackalloc|nameof|async|await|volatile|unchecked|checked|unsafe|fixed)\b",
            new ColorStyle(0.4f, 0.8f, 0.8f));
        
        // Keyword
        language.AddPattern(@"\b(get|set)\b",
            new ColorStyle(0.7f, 0.8f, 0.3f));

        return language;
    }
}