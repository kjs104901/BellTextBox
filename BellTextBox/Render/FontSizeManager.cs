using System.Numerics;
using Bell.Data;

namespace Bell.Render;

public class FontSizeManager
{
    private readonly TextBox _textBox;
    private readonly Dictionary<Vector2, Dictionary<char, float>> _sizeCacheDictionary = new();

    private Vector2 _referenceSize;
    
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
        _referenceSize = _textBox.GetCharRenderSize('#');
        _sizeCacheDictionary.TryAdd(_referenceSize, new Dictionary<char, float>());
        _sizeWidthCache = _sizeCacheDictionary[_referenceSize];

        _sizeHeight = _referenceSize.Y;
    }

    public float GetFontReferenceWidth()
    {
        return _referenceSize.X;
    }

    public float GetFontWidth(char c)
    {
        // TODO handle \t width?
        
        if (false == _sizeWidthCache.TryGetValue(c, out float fontWidth))
        {
            var renderSize = _textBox.GetCharRenderSize(c);

            fontWidth = renderSize.X;
            _sizeWidthCache[c] = fontWidth;
            _sizeHeight = Math.Max(_sizeHeight, renderSize.Y);
        }
        return fontWidth;
    }

    public float GetFontHeight()
    {
        return _sizeHeight;
    }
}