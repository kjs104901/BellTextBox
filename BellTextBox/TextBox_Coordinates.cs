using System.Numerics;
using Bell.Data;

namespace Bell;

public partial class TextBox
{
    public PageCoordinates ViewToPage(Vector2 viewCoordinates, int yOffset = 0)
    {
        float x = viewCoordinates.X - LineNumberWidth - FoldWidth;
        float y = viewCoordinates.Y;
        
        PageCoordinates pageCoordinates = new();

        int row = (int)(y / GetFontHeight()) + yOffset;
        if (row < 0)
            row = 0;
        if (row >= SubLines.Count)
            row = SubLines.Count - 1;

        if (x < -FoldWidth)
        {
            pageCoordinates.IsLineNumber = true;
            pageCoordinates.Column = 0;
        }
        else if (SubLines.Count > row)
        {
            SubLine subLine = SubLines[row];
            
            if (x < -FoldWidth * 0.2 && subLine.Folding != null)
            {
                pageCoordinates.IsFold = true;
                pageCoordinates.Column = 0;
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
                pageCoordinates.Column = column;
            }
        }

        pageCoordinates.Row = row;

        return pageCoordinates;
    }

    public TextCoordinates PageToText(PageCoordinates pageCoordinates)
    {
        throw new NotImplementedException();
    }
    
    public PageCoordinates TextToPage(TextCoordinates textCoordinates)
    {
        throw new NotImplementedException();
    }

    public Vector2 PageToView(PageCoordinates pageCoordinates)
    {
        var view = new Vector2();

        if (SubLines.Count > pageCoordinates.Row)
        {
            SubLine subLine = SubLines[pageCoordinates.Row];

            int column = pageCoordinates.Column;
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
        view.Y = pageCoordinates.Row * GetFontHeight();

        return view;
    }
}