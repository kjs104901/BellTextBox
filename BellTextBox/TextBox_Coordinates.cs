using System.Numerics;
using Bell.Data;

namespace Bell;

public partial class TextBox
{
    private void ConvertCoordinates(Vector2 viewCoordinates,
        out int rowIndex,
        out TextCoordinates textCoordinates,
        int yOffset = 0)
    {
        float x = viewCoordinates.X - LineNumberWidth - FoldWidth;
        float y = viewCoordinates.Y;
        
        textCoordinates = new();
        
        int row = (int)(y / GetFontHeight()) + yOffset;
        if (row < 0)
            row = 0;
        if (row >= Rows.Count)
            row = Rows.Count - 1;
        
        rowIndex = row;
        
        if (x < -FoldWidth)
        {
            textCoordinates.IsLineNumber = true;
            return;
        }
        
        Line? line = null;
        if (Rows.Count > row)
        {
            SubLine subLine = Rows[row];
            textCoordinates.Line = subLine.Line;

            if (x < - GetFontWhiteSpaceWidth())
            {
                if (subLine.Line.Folding != null)
                {
                    textCoordinates.IsFold = true;
                    return;
                }
            }

            int column = 0;
            float pageX = x - subLine.WrapIndentWidth;
            foreach (var textBlockRender in subLine.TextBlockRenders)
            {
                if (pageX < textBlockRender.Width)
                {
                    foreach (char c in textBlockRender.Text)
                    {
                        var fontWidth = GetFontWidth(c);

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
            
            textCoordinates.CharIndex = 0;
            foreach (SubLine lineSubLine in subLine.Line.SubLines)
            {
                if (subLine.WrapIndex <= lineSubLine.WrapIndex)
                    break;
                textCoordinates.CharIndex += lineSubLine.Chars.Count;
            }
            textCoordinates.CharIndex += column;
        }
    }
    
    /*
    private Vector2 PageToView(TextCoordinates textCoordinates)
    {
        var view = new Vector2();

        if (Rows.Count > textCoordinates.Row)
        {
            SubLine subLine = Rows[textCoordinates.Row];

            int column = textCoordinates.Column;
            float pageX = 0.0f + subLine.TabWidth;
            foreach (var textBlockRender in subLine.TextBlockRenders)
            {
                if (column < textBlockRender.Text.Length)
                {
                    foreach (char c in textBlockRender.Text)
                    {
                        if (column < 1)
                            break;

                        column -= 1;
                        pageX += GetFontWidth(c);
                    }

                    break;
                }

                column -= textBlockRender.Text.Length;
                pageX += textBlockRender.Width;
            }

            view.X = Math.Max(view.X, pageX);
        }

        view.X += LineNumberWidth + FoldWidth;
        view.Y = textCoordinates.Row * GetFontHeight();

        return view;
    }
    */
}