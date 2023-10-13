using System.Numerics;
using Bell.Data;

namespace Bell.Render;

public class FontSizeManager
{
    private readonly TextBox _textBox;
    private readonly Dictionary<float, Dictionary<char, float>> _sizeCacheDictionary = new();

    private Dictionary<char, float> _sizeWidthCache;
    private float _sizeHeight;
    
    public FontSizeManager(TextBox textBox)
    {
        _textBox = textBox;
        _sizeWidthCache = new();
        UpdateReferenceSize();
    }

    public void UpdateReferenceSize()
    {
        var fontSize = _textBox.GetFontSize();
        _sizeCacheDictionary.TryAdd(fontSize, new Dictionary<char, float>());
        _sizeWidthCache = _sizeCacheDictionary[fontSize];
        _sizeHeight = fontSize;
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
            fontWidth = _textBox.GetCharWidth(c);
            _sizeWidthCache[c] = fontWidth;
        }
        return fontWidth;
    }

    public float GetFontSize()
    {
        return _sizeHeight;
    }
}