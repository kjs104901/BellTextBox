using Bell.Data;
using Bell.Languages;

namespace Bell;

public partial class TextBox
{
    public ColorStyle PageBackgroundColor = new ColorStyle(0.2f, 0.1f, 0.1f, 1.0f);
    
    public ColorStyle SelectedBackgroundColor = new ColorStyle(0.3f, 0.4f, 0.3f, 0.5f);
    public ColorStyle WhiteSpaceFontColor = new ColorStyle(0.3f, 0.4f, 0.1f, 1.0f);
    
    public ColorStyle UiTextColor = new ColorStyle(0.4f, 0.8f, 0.2f, 1.0f);
    
    public ColorStyle CaretColor = new ColorStyle(0.8f, 0.5f, 0.2f, 1.0f);
    public ColorStyle ImeInputColor = new ColorStyle(0.7f, 0.5f, 0.5f, 1.0f);
}