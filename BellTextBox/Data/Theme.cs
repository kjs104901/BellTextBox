namespace Bell.Data;

public class Theme
{
    public ColorStyle DefaultFontColor;
    public ColorStyle LineCommentFontColor;
    public ColorStyle BlockCommentFontColor;
    
    public ColorStyle LineSelectedBackgroundColor;
}

public class DarkTheme : Theme
{
    public DarkTheme()
    {
        DefaultFontColor = new ColorStyle(0.4f, 0.8f, 0.2f, 1.0f);
        LineCommentFontColor = new ColorStyle(0.3f, 0.4f, 0.8f, 1.0f);
        BlockCommentFontColor = new ColorStyle(0.9f, 0.1f, 0.1f, 1.0f);

        LineSelectedBackgroundColor = new ColorStyle(0.3f, 0.4f, 0.3f, 0.5f);
    }
}

public class LightTheme : Theme
{
    public LightTheme()
    {
    }
}