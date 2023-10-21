namespace Bell;

public partial class TextBox
{
    private readonly Dictionary<float, Dictionary<char, float>> _sizeCacheDictionary = new();

    private Dictionary<char, float> _sizeWidthCache = new();

    public void UpdateReferenceSize()
    {
        var fontSize = _backend.GetFontSize();
        if (false == _sizeCacheDictionary.ContainsKey(fontSize))
            _sizeCacheDictionary.TryAdd(fontSize, new Dictionary<char, float>());
        _sizeWidthCache = _sizeCacheDictionary[fontSize];
    }

    public float GetFontReferenceWidth()
    {
        return GetFontWidth('#');
    }

    public float GetFontWidth(char c)
    {
        // TODO handle \t width?

        if (false == _sizeWidthCache.TryGetValue(c, out float fontWidth))
        {
            fontWidth = _backend.GetCharWidth(c);
            _sizeWidthCache[c] = fontWidth;
        }

        return fontWidth;
    }

    public float GetFontHeight()
    {
        return _backend.GetFontSize() * LeadingHeight;
    }

    public float GetFontHeightOffset()
    {
        return ((_backend.GetFontSize() * LeadingHeight) - _backend.GetFontSize()) / 2.0f;
    }
}