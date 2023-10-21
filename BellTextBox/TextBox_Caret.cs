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
}

public partial class TextBox
{
    public readonly List<Caret> Carets = new();
    
    public void SetCaretDirty()
    {
        foreach (SubLine subLine in Rows)
        {
            subLine.LineSelectionCache.SetDirty();
        }
    }

    public Caret SingleCaret(TextCoordinates textCoordinates = new())
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

    public void AddCaret(TextCoordinates textCoordinates)
    {
        Carets.Add(new Caret() { Position = textCoordinates, AnchorPosition = textCoordinates });
        SetCaretDirty();
    }

    public void MoveCaretsPosition(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
        SetCaretDirty();
    }
    
    public void MoveCaretsAnchor(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
        SetCaretDirty();
    }

    public void RemoveCaretsSelection()
    {
        foreach (Caret caret in Carets)
        {
            caret.AnchorPosition = caret.Position;
        }
        SetCaretDirty();
    }

    public void SelectRectangle(TextCoordinates startPosition, TextCoordinates endPosition)
    {
        Carets.Clear();
        // TODO select multiple lines
        SetCaretDirty();
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


        ActionManager.DoAction(new PasteAction(text));
        */
    }
}