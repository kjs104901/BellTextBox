using System.Numerics;
using Bell.Data;

namespace Bell;

public partial class TextBox
{
    private TextCoordinates ViewToText(Vector2 viewCoordinates, int yOffset = 0)
    {
        float x = viewCoordinates.X - LineNumberWidth - FoldWidth;
        float y = viewCoordinates.Y;
        
        TextCoordinates textCoordinates = new();

        int row = (int)(y / GetFontHeight()) + yOffset;
        if (row < 0)
            row = 0;
        if (row >= SubLines.Count)
            row = SubLines.Count - 1;

        if (x < -FoldWidth)
        {
            textCoordinates.IsLineNumber = true;
            textCoordinates.Column = 0;
        }
        else if (SubLines.Count > row)
        {
            SubLine subLine = SubLines[row];
            
            if (x < -FoldWidth * 0.2 && subLine.Folding != null)
            {
                textCoordinates.IsFold = true;
                textCoordinates.Column = 0;
            }
            else
            {
                int column = 0;
                float pageX = x - subLine.TabWidth;
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
                textCoordinates.Column = column;
            }

            textCoordinates.LineIndex = subLine.LineIndex;
        }

        textCoordinates.Row = row;

        return textCoordinates;
    }

    private Vector2 PageToView(TextCoordinates textCoordinates)
    {
        var view = new Vector2();

        if (SubLines.Count > textCoordinates.Row)
        {
            SubLine subLine = SubLines[textCoordinates.Row];

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
}