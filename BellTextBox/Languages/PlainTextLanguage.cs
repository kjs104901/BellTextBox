namespace Bell.Languages;

public partial class Language
{
    public static Language PlainText()
    {
        Language language = new();
        
        language.AddFolding("{", "}");
        language.AddLineComment("//");
        language.AddString("\"");
        
        return language;
    }
}