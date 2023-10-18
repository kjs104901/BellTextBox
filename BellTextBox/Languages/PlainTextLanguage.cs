namespace Bell.Languages;

public partial class Language
{
    public static Language PlainText()
    {
        Language language = new();
        
        language.Foldings.Add(("{", "}"));
        
        return language;
    }
}