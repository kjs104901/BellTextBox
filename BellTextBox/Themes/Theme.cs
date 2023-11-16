namespace Bell.Themes;

public class Theme
{
    // Background
    public ColorStyle Background;
    public ColorStyle BackgroundSelection;
    public ColorStyle BackgroundMenu;

    // Foreground
    public ColorStyle Foreground;
    public ColorStyle ForegroundDimmed;

    // Token colors
    public enum Token
    {
        Comment,
        Constant,
        Attribute,
        Invalid,
        String,
        Keyword,
        KeywordControl,
        Function,
        Namespace,
        Type,
        Numeric,
        Variable
    }
    public Dictionary<Token, ColorStyle> TokenColors = new();

    public static Theme Dark()
    {
        return new Theme()
        {
            Background            = new ("#1E1E1E"),
            BackgroundSelection   = new ("#ADD6FF26"),
            BackgroundMenu        = new ("#252525"),
            
            Foreground            = new ("#D4D4D4"),
            ForegroundDimmed      = new ("#A6A6A6"),
            
            TokenColors = new Dictionary<Token, ColorStyle>()
            {
                { Token.Comment,           new ColorStyle("#6A9955") },
                { Token.Constant,          new ColorStyle("#569CD6") },
                { Token.Attribute,         new ColorStyle("#D7BA7D") },
                { Token.Invalid,           new ColorStyle("#F44747") },
                { Token.String,            new ColorStyle("#CE9178") },
                { Token.Keyword,           new ColorStyle("#569CD6") },
                { Token.KeywordControl,    new ColorStyle("#C586C0") },
                { Token.Function,          new ColorStyle("#DCDCAA") },
                { Token.Namespace,         new ColorStyle("#4EC9B0") },
                { Token.Type,              new ColorStyle("#4EC9B0") },
                { Token.Numeric,           new ColorStyle("#B5CEA8") },
                { Token.Variable,          new ColorStyle("#9CDCFE") }
            }
        };
    }
}