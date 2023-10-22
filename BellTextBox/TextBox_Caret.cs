using System.Numerics;
using Bell.Data;
using Bell.Utils;

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
}

public partial class TextBox
{
    public readonly List<Caret> Carets = new();

    private void SetCaretDirty()
    {
        foreach (SubLine row in Rows)
        {
            row.LineSelectionCache.SetDirty();
        }
    }

    private Caret SingleCaret(TextCoordinates textCoordinates = new())
    {
        if (Carets.Count > 1)
            Carets.RemoveRange(1, Carets.Count - 1);

        if (Carets.Count > 0)
        {
            Carets[0].Position = textCoordinates;
            Carets[0].AnchorPosition = textCoordinates;
        }
        else
        {
            Carets.Add(new Caret() { Position = textCoordinates, AnchorPosition = textCoordinates });
        }

        SetCaretDirty();
        return Carets[0];
    }

    private void AddCaret(TextCoordinates textCoordinates)
    {
        Carets.Add(new Caret() { Position = textCoordinates, AnchorPosition = textCoordinates });
        SetCaretDirty();
    }

    public void SetCarets(List<Caret> carets)
    {
        Carets.Clear();
        Carets.AddRange(carets);
        SetCaretDirty();
    }

    public void MoveCaretsPosition(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            caret.Position = FindCoordinates(caret.Position, caretMove);
        }
        SetCaretDirty();
    }
    
    public void MoveCaretsAnchor(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            caret.AnchorPosition = FindCoordinates(caret.AnchorPosition, caretMove);
        }
        SetCaretDirty();
    }
    
    private TextCoordinates FindCoordinates(TextCoordinates currentCoordinates, CaretMove caretMove)
    {
        TextCoordinates newCoordinates = currentCoordinates;
        
        if (ThreadLocal.TextBox.GetLine(currentCoordinates.LineIndex, out Line line))
        {
            if (CaretMove.Right == caretMove)
            {
                // Check end of line
                if (currentCoordinates.CharIndex < line.Chars.Count)
                {
                    newCoordinates.CharIndex++;
                }
                else
                {
                    // Check end of file
                    if (currentCoordinates.LineIndex < ThreadLocal.TextBox.Lines.Count - 1)
                    {
                        newCoordinates.LineIndex++;
                        newCoordinates.CharIndex = 0;
                    }
                }
            }
            else if (CaretMove.Left == caretMove)
            {
                // Check start of line
                if (currentCoordinates.CharIndex > 0)
                {
                    newCoordinates.CharIndex--;
                }
                else
                {
                    // Check start of file
                    if (currentCoordinates.LineIndex > 0)
                    {
                        newCoordinates.LineIndex--;
                        newCoordinates.CharIndex = ThreadLocal.TextBox.Lines[newCoordinates.LineIndex].Chars.Count;
                    }
                }
            }
            else if (CaretMove.Up == caretMove)
            {
                // Check start of file
                if (currentCoordinates.LineIndex > 0)
                {
                    newCoordinates.LineIndex--;
                    newCoordinates.CharIndex = Math.Min(currentCoordinates.CharIndex, ThreadLocal.TextBox.Lines[newCoordinates.LineIndex].Chars.Count);
                }
            }
            else if (CaretMove.Down == caretMove)
            {
                // Check end of file
                if (currentCoordinates.LineIndex < ThreadLocal.TextBox.Lines.Count - 1)
                {
                    newCoordinates.LineIndex++;
                    newCoordinates.CharIndex = Math.Min(currentCoordinates.CharIndex, ThreadLocal.TextBox.Lines[newCoordinates.LineIndex].Chars.Count);
                }
            }
            else if (CaretMove.StartOfLine == caretMove)
            {
                newCoordinates.CharIndex = 0;
            }
            else if (CaretMove.EndOfLine == caretMove)
            {
                newCoordinates.CharIndex = line.Chars.Count;
            }
            else if (CaretMove.StartOfWord == caretMove)
            {
                // TODO
            }
            else if (CaretMove.EndOfWord == caretMove)
            {
                // TODO
            }
            else if (CaretMove.StartOfFile == caretMove)
            {
                newCoordinates.LineIndex = 0;
                newCoordinates.CharIndex = 0;
            }
            else if (CaretMove.EndOfFile == caretMove)
            {
                newCoordinates.LineIndex = ThreadLocal.TextBox.Lines.Count - 1;
                newCoordinates.CharIndex = ThreadLocal.TextBox.Lines[newCoordinates.LineIndex].Chars.Count;
            }
            else if (CaretMove.PageUp == caretMove)
            {
                // TODO
            }
            else if (CaretMove.PageDown == caretMove)
            {
                // TODO
            }
        }
        
        return newCoordinates;
    }

    private bool HasCaretsSelection()
    {
        foreach (Caret caret in Carets)
        {
            if (caret.HasSelection)
                return true;
        }
        return false;
    }

    public void RemoveCaretsSelection()
    {
        foreach (Caret caret in Carets)
        {
            caret.AnchorPosition = caret.Position;
        }
        SetCaretDirty();
    }

    private void SelectRectangle(TextCoordinates startPosition, TextCoordinates endPosition)
    {
        Carets.Clear();
        // TODO select multiple lines
        SetCaretDirty();
    }

    private void CopyClipboard()
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

    private void PasteClipboard()
    {
        /*
        if (CaretManager.Carets.Count == 0)
            return;

        string text = GetClipboard();
        if (text == null)
            return;

        // if caret number is same as text line number, paste to each line


        ActionManager.DoAction(new PasteAction(text));
        */
    }
}