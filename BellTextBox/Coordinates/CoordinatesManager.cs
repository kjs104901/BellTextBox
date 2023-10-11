namespace Bell.Coordinates;

public class CoordinatesManager : IDisposable
{
    private TextBox? _textBox;

    public CoordinatesManager(TextBox? textBox)
    {
        _textBox = textBox;
    }

    public PageCoordinates Convert(ViewCoordinates viewCoordinates)
    {
        PageCoordinates pageCoordinates = new();
        if (null == _textBox || null == _textBox.Text || null == _textBox.FontSizeManager)
            return pageCoordinates;

        pageCoordinates.X = viewCoordinates.X - _textBox.LineNumberWidth - _textBox.MarkerWidth;
        pageCoordinates.Y = viewCoordinates.Y;

        return pageCoordinates;
    }

    public TextCoordinates Convert(PageCoordinates pageCoordinates, int offset = 0)
    {
        TextCoordinates textCoordinates = new();
        if (null == _textBox || null == _textBox.Text || null == _textBox.FontSizeManager)
            return textCoordinates;
        
        int row = (int)(pageCoordinates.Y / _textBox.FontSizeManager.GetFontHeight()) + offset;
        if (row < 0)
            row = 0;
        if (row >= _textBox.Text.LineViews.Count)
            row = _textBox.Text.LineViews.Count - 1;

        textCoordinates.Row = row;

        if (pageCoordinates.X < -_textBox.LineNumberWidth)
        {
            textCoordinates.IsLineNumber = true;
            textCoordinates.Column = -2;
        }
        else if (pageCoordinates.X < 0.0f)
        {
            textCoordinates.IsMarker = true;
            textCoordinates.Column = -1;
        }
        else
        {
            textCoordinates.Column = 0; //TODO
        }
        
        return textCoordinates;
    }
    
    public void Dispose()
    {
        _textBox = null;
    }
}