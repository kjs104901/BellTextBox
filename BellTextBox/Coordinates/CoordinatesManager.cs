using Bell.Data;
using Bell.Render;

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
            LineView lineView = _textBox.Text.LineViews[row];
            Line line = _textBox.Text.Lines[lineView.LineIndex];
            LineRender lineRender = line.LineRenders[lineView.RenderIndex];

            int column = 0;
            float pageX = pageCoordinates.X;
            foreach (var textBlockRender in lineRender.TextBlockRenders)
            {
                if (pageX < textBlockRender.Width)
                {
                    foreach (char c in textBlockRender.Text)
                    {
                        var fontWidth = _textBox.FontSizeManager.GetFontWidth(c);
                        
                        if (pageX < fontWidth)
                            break;

                        column += 1;
                        pageX -= fontWidth;
                    }
                    break;
                }

                column += textBlockRender.Text.Length;
                pageX -= textBlockRender.Width;
            }
            
            textCoordinates.Column = column;
        }
        
        textCoordinates.Row = row;
        
        return textCoordinates;
    }
    
    public void Dispose()
    {
        _textBox = null;
    }
}