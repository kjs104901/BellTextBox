namespace Bell;

public partial class TextBox
{
    private readonly Dictionary<float, Dictionary<char, float>> _sizeCacheDictionary = new();

    private Dictionary<char, float> _sizeWidthCache = new();
    
    public void UpdateReferenceSize()
    {
        var fontSize = GetFontSize();
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
            fontWidth = GetCharWidth(c);
            _sizeWidthCache[c] = fontWidth;
        }
        return fontWidth;
    }
}