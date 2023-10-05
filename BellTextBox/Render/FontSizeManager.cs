using Bell.Data;

namespace Bell.Render;

public class FontSizeManager
{
    private readonly TextBox _textBox;
    private readonly Dictionary<RectSize, Dictionary<char, float>> _sizeCacheDictionary = new();

    private RectSize _referenceSize;
    
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
        _referenceSize = _textBox.TextBoxBackend.GetRenderSize('#');
        _sizeCacheDictionary.TryAdd(_referenceSize, new Dictionary<char, float>());
        _sizeWidthCache = _sizeCacheDictionary[_referenceSize];

        _sizeHeight = _referenceSize.Height;
    }

    public float GetFontReferenceWidth()
    {
        return _referenceSize.Width;
    }

    public float GetFontWidth(char c)
    {
        if (false == _sizeWidthCache.TryGetValue(c, out float fontWidth))
        {
            var rectSize = _textBox.TextBoxBackend.GetRenderSize(c);

            fontWidth = rectSize.Width;
            _sizeWidthCache[c] = fontWidth;
            _sizeHeight = Math.Max(_sizeHeight, rectSize.Height);
        }
        return fontWidth;
    }

    public float GetFontHeight()
    {
        return _sizeHeight;
    }
}