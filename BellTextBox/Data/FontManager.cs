using Bell.Utils;

namespace Bell.Data;

public class FontManager
{
    private readonly Dictionary<float, Font> _fontDictionary = new();

    private Font _fontCache = new(10.0f);

    public void UpdateReferenceSize()
    {
        var fontSize = ThreadLocal.TextBox._backend.GetFontSize();
        if (false == _fontDictionary.ContainsKey(fontSize))
            _fontDictionary.TryAdd(fontSize, new Font(fontSize));
        _fontCache = _fontDictionary[fontSize];
    }

    public float GetFontReferenceWidth() => GetFontWidth('#');
    public float GetFontWhiteSpaceWidth() => GetFontWidth(' ');

    public float GetFontWidth(char c)
    {
        // TODO handle \t width?
        return _fontCache.GetFontWidth(c);
    }

    public float GetLineHeight()
    {
        return _fontCache.Size * ThreadLocal.TextBox.LeadingHeight;
    }

    public float GetLineHeightOffset()
    {
        return ((_fontCache.Size * ThreadLocal.TextBox.LeadingHeight) - _fontCache.Size) / 2.0f;
    }
}