using Bell.Data;

namespace Bell.Render;

public class FontSizeManager
{
    private readonly TextBox _textBox;
    private readonly Dictionary<RectSize, Dictionary<char, RectSize>> _sizeCacheDictionary = new();

    private RectSize _referenceSize;
    private Dictionary<char, RectSize> _sizeCache;
    
    public FontSizeManager(TextBox textBox)
    {
        _textBox = textBox;
        _sizeCache = new();
    }

    public void UpdateReferenceSize()
    {
        _referenceSize = _textBox.TextBoxBackend.GetRenderSize('#');
        _sizeCacheDictionary.TryAdd(_referenceSize, new Dictionary<char, RectSize>());
        _sizeCache = _sizeCacheDictionary[_referenceSize];
    }

    public RectSize GetFonRectSize(char c)
    {
        if (false == _sizeCache.TryGetValue(c, out RectSize rectSize))
        {
            rectSize = _textBox.TextBoxBackend.GetRenderSize(c);
            _sizeCache[c] = rectSize;
        }
        return rectSize;
    }
}