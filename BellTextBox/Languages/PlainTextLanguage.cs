namespace Bell.Languages;

public partial class Language
{
    public static Language PlainText()
    {
        Language language = new();
        
        language.AddFolding("{", "}");
        language.AddLineComment("//");
        language.AddString("\"");
        
        language.DefaultStyle = new ColorStyle(0.4f, 0.8f, 0.2f, 1.0f);
        language.CommentStyle = new ColorStyle(0.3f, 0.5f, 0.2f, 1.0f);
        language.StringStyle = new ColorStyle(0.2f, 0.3f, 0.3f, 1.0f);
        
        return language;
    }
}