using System.Numerics;
using Bell.Data;
using Bell.Render;

namespace Bell.Coordinates;

public class CoordinatesManager
{
    private readonly TextBox _textBox;

    public CoordinatesManager(TextBox textBox)
    {
        _textBox = textBox;
    }

    public Vector2 ViewToPage(Vector2 viewCoordinates)
    {
        Vector2 pageCoordinates = new()
        {
            X = viewCoordinates.X - _textBox.LineNumberWidth - _textBox.FoldWidth,
            Y = viewCoordinates.Y
        };
        return pageCoordinates;
    }

    public TextCoordinates PageToText(Vector2 pageCoordinates, int offset = 0)
    {
        TextCoordinates textCoordinates = new();
        
        int row = (int)(pageCoordinates.Y / _textBox.FontSizeManager.GetFontSize()) + offset;
        if (row < 0)
            row = 0;
        if (row >= _textBox.Text.LineWraps.Count)
            row = _textBox.Text.LineWraps.Count - 1;
        
        if (pageCoordinates.X < -_textBox.LineNumberWidth)
        {
            textCoordinates.IsLineNumber = true;
            textCoordinates.Column = 0;
        }
        else if (pageCoordinates.X < 0.0f)
        {
            textCoordinates.IsFold = true;
            textCoordinates.Column = 0;
        }
        else
        {
            LineWrap lineWrap = _textBox.Text.LineWraps[row];
            Line line = _textBox.Text.Lines[lineWrap.LineIndex];
            LineRender lineRender = line.LineRenders[lineWrap.RenderIndex];

            int column = 0;
            float pageX = pageCoordinates.X;
            foreach (var textBlockRender in lineRender.TextBlockRenders)
            {
                if (pageX < textBlockRender.Width)
                {
                    foreach (char c in textBlockRender.Text)
                    {
                        var fontWidth = _textBox.FontSizeManager.GetFontWidth(c);
                        
                        if (pageX < fontWidth * 0.5)
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

    public Vector2 TextToPage(TextCoordinates textCoordinates)
    {
        var pageCoordinates = new Vector2();

        pageCoordinates.X = 0;

        if (_textBox.Text.LineWraps.Count > textCoordinates.Row)
        {
            LineWrap lineWrap = _textBox.Text.LineWraps[textCoordinates.Row];
            Line line = _textBox.Text.Lines[lineWrap.LineIndex];
            LineRender lineRender = line.LineRenders[lineWrap.RenderIndex];

            
            int column = textCoordinates.Column;
            float pageX = 0.0f;
            foreach (var textBlockRender in lineRender.TextBlockRenders)
            {
                if (column < textBlockRender.Text.Length)
                {
                    foreach (char c in textBlockRender.Text)
                    {
                        if (column < 1)
                            break;
                        
                        column -= 1;
                        pageX += _textBox.FontSizeManager.GetFontWidth(c);
                    }
                    break;
                }
                column -= textBlockRender.Text.Length;
                pageX += textBlockRender.Width;
            }
            pageCoordinates.X = pageX;
        }
        
        pageCoordinates.Y = textCoordinates.Row * _textBox.FontSizeManager.GetFontSize();

        return pageCoordinates;
    }

    public Vector2 PageToView(Vector2 pageCoordinates)
    {
        Vector2 viewCoordinates = new()
        {
            X = pageCoordinates.X + _textBox.LineNumberWidth + _textBox.FoldWidth,
            Y = pageCoordinates.Y
        };
        return viewCoordinates;
    }
}