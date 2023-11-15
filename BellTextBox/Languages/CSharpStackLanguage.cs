using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public static Language CSharpStack()
    {
        Language language = new()
        {
            DefaultStyle = new ColorStyle(0.4f, 0.8f, 0.2f),
            CommentStyle = new ColorStyle(0.3f, 0.5f, 0.2f),
            StringStyle = new ColorStyle(0.2f, 0.3f, 0.3f)
        };

        // File name
        language.AddPattern(@"(?<=in )(?<file>.*?)(?=:line)",
            new ColorStyle(0.5f, 0.4f, 0.8f));
        
        // Line info
        language.AddPattern(@":line\s+\d+",
            new ColorStyle(0.8f, 0.4f, 0.2f));
        
        // Namespace
        language.AddPattern(@"(?<=at\s+)([\w.]+)(?=\.[^.(]+)",
            new ColorStyle(0.8f, 0.8f, 0.2f));
        
        // Function
        language.AddPattern(@"(?:\.|>)([^>.]+)(?=\()",
            new ColorStyle(0.4f, 0.8f, 0.8f));
        
        // Param
        language.AddPattern( @"\(([^)]+)\)",
            new ColorStyle(0.7f, 0.8f, 0.3f));

        return language;
    }
}