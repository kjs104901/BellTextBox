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
    
    private bool _caretChanged;

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

        _caretChanged = true;
        return Carets[0];
    }

    public void AddCaret(TextCoordinates textCoordinates)
    {
        Carets.Add(new Caret() { Position = textCoordinates, Selection = textCoordinates });
        _caretChanged = true;
    }

    public void MoveCaretsPosition(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
        _caretChanged = true;
    }

    public void MoveCaretsSelection(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
        _caretChanged = true;
    }

    public void SelectRectangle(Vector2 startPosition, Vector2 endPosition)
    {
        Carets.Clear();
        // TODO select multiple lines
        _caretChanged = true;
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
            X = viewCoordinates.X - LineNumberWidth - FoldWidth,
            Y = viewCoordinates.Y
        };
        return pageCoordinates;
    }

    public TextCoordinates PageToText(Vector2 pageCoordinates, int yOffset = 0)
    {
        TextCoordinates textCoordinates = new();

        int row = (int)(pageCoordinates.Y / GetFontHeight()) + yOffset;
        if (row < 0)
            row = 0;
        if (row >= SubLines.Count)
            row = SubLines.Count - 1;

        if (pageCoordinates.X < -FoldWidth)
        {
            textCoordinates.IsLineNumber = true;
            textCoordinates.Column = 0;
        }
        else if (SubLines.Count > row)
        {
            SubLine subLine = SubLines[row];
            
            if (pageCoordinates.X < -FoldWidth * 0.2 && subLine.Folding != null)
            {
                textCoordinates.IsFold = true;
                textCoordinates.Column = 0;
            }
            else
            {
                int column = 0;
                float pageX = pageCoordinates.X - subLine.TabWidth;
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
        }

        textCoordinates.Row = row;

        return textCoordinates;
    }

    public Vector2 TextToPage(TextCoordinates textCoordinates)
    {
        var pageCoordinates = new Vector2();

        pageCoordinates.X = 0;

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

            pageCoordinates.X = pageX;
        }

        pageCoordinates.Y = textCoordinates.Row * GetFontHeight();

        return pageCoordinates;
    }

    public Vector2 PageToView(Vector2 pageCoordinates)
    {
        Vector2 viewCoordinates = new()
        {
            X = pageCoordinates.X + LineNumberWidth + FoldWidth,
            Y = pageCoordinates.Y
        };
        return viewCoordinates;
    }
}