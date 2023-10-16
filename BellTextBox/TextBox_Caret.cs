using System.Numerics;
using Bell.Data;

namespace Bell;

public enum CaretMove
{
    None,
    
    Up,
    Down,
    Left,
    Right,
    
    StartOfFile,
    EndOfFile,
    StartOfLine,
    EndOfLine,
    StartOfWord,
    EndOfWord,
    
    PageUp,
    PageDown,
    
    Position,
    Selection
}

public partial class TextBox
{
    public readonly List<Caret> Carets = new();
    
    public Caret SingleCaret(TextCoordinates textCoordinates = new())
    {
        if (Carets.Count > 1)
            Carets.RemoveRange(1, Carets.Count - 1);

        if (Carets.Count > 0)
        {
            Carets[0].Position = textCoordinates;
            Carets[0].Selection = textCoordinates;
        }
        else
        {
            Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
        }
        
        return Carets[0];
    }

    public void AddCaret(TextCoordinates textCoordinates)
    {
        Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
    }

    public void MoveCaretsPosition(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
    }
    
    public void MoveCaretsSelection(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
    }

    public void SelectRectangle(Vector2 startPosition, Vector2 endPosition)
    {
        Carets.Clear();
        // TODO select multiple lines
    }

    public void CopyClipboard()
    {
        /*
        if (CaretManager.Carets.Count == 0)
            return;

        StringBuilder.Clear();
        foreach (Caret caret in CaretManager.Carets)
        {
            if (caret.HasSelection)
            {
                StringBuilder.Append(Text.GetText(caret.SelectionStart, caret.SelectionEnd));
            }
        }

        SetClipboard(StringBuilder.ToString());
        */
    }

    public void PasteClipboard()
    {
        /*
        if (CaretManager.Carets.Count == 0)
            return;

        string text = GetClipboard();
        if (text == null)
            return;

        // if caret number is same as text line number, paste to each line


        ActionManager.DoAction(new PasteAction(this, text));
        */
    }
    
    public Vector2 ViewToPage(Vector2 viewCoordinates)
    {
        Vector2 pageCoordinates = new()
        {
            X = viewCoordinates.X - LineNumberWidthMax - FoldWidth,
            Y = viewCoordinates.Y
        };
        return pageCoordinates;
    }

    public TextCoordinates PageToText(Vector2 pageCoordinates, int yOffset = 0)
    {
        TextCoordinates textCoordinates = new();
        
        int row = (int)(pageCoordinates.Y / GetFontSize()) + yOffset;
        if (row < 0)
            row = 0;
        if (row >= Text.LineRenders.Count)
            row = Text.LineRenders.Count - 1;
        
        if (pageCoordinates.X < -FoldWidth)
        {
            textCoordinates.IsLineNumber = true;
            textCoordinates.Column = 0;
        }
        else if (pageCoordinates.X < 0.0f)
        {
            textCoordinates.IsFold = true;
            textCoordinates.Column = 0;
        }
        else if (Text.LineRenders.Count > row)
        {
            LineRender lineRender = Text.LineRenders[row];

            int column = 0;
            float pageX = pageCoordinates.X;
            foreach (var textBlockRender in lineRender.TextBlockRenders)
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
        
        textCoordinates.Row = row;
        
        return textCoordinates;
    }

    public Vector2 TextToPage(TextCoordinates textCoordinates)
    {
        var pageCoordinates = new Vector2();

        pageCoordinates.X = 0;

        if (Text.LineRenders.Count > textCoordinates.Row)
        {
            LineRender lineRender = Text.LineRenders[textCoordinates.Row];
            
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
                        pageX += GetFontWidth(c);
                    }
                    break;
                }
                column -= textBlockRender.Text.Length;
                pageX += textBlockRender.Width;
            }
            pageCoordinates.X = pageX;
        }
        
        pageCoordinates.Y = textCoordinates.Row * GetFontSize();

        return pageCoordinates;
    }

    public Vector2 PageToView(Vector2 pageCoordinates)
    {
        Vector2 viewCoordinates = new()
        {
            X = pageCoordinates.X + LineNumberWidthMax + FoldWidth,
            Y = pageCoordinates.Y
        };
        return viewCoordinates;
    }
}