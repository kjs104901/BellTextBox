using Bell.Coordinates;

namespace Bell.Data;

public class Page
{
    private readonly TextBox _textBox;
    
    public readonly Text Text;

    public RectSize Size => _sizeCache.Get();
    private readonly Cache<RectSize> _sizeCache;
    
    // View
    private ViewCoordinates _viewStart = new ();
    private ViewCoordinates _viewEnd = new ();

    private uint _viewLineStart = 0;
    private uint _viewLineEnd = 0;
    private bool _viewLineDirty = false;

    public Page(TextBox textBox)
    {
        _textBox = textBox;
        Text = new Text(_textBox);

        _sizeCache = new Cache<RectSize>(new RectSize(), UpdateSize);
    }
    
    public void UpdateView(ViewCoordinates start, ViewCoordinates end)
    {
        _viewStart = start;
        _viewEnd = end;
        
        _viewLineDirty = true;
    }

    private RectSize UpdateSize(RectSize _)
    {
        return new();
    }
}