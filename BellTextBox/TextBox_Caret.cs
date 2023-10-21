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
    
    public void SetCaretDirty()
    {
        foreach (SubLine subLine in SubLines)
        {
            subLine.LineSelectionCache.SetDirty();
        }
    }

    public Caret SingleCaret(PageCoordinates pageCoordinates = new())
    {
        if (Carets.Count > 1)
            Carets.RemoveRange(1, Carets.Count - 1);

        if (Carets.Count > 0)
        {
            Carets[0].Position = pageCoordinates;
            Carets[0].Selection = pageCoordinates;
        }
        else
        {
            Carets.Add(new Caret() { Position = pageCoordinates, Selection = pageCoordinates });
        }

        SetCaretDirty();
        return Carets[0];
    }

    public void AddCaret(PageCoordinates pageCoordinates)
    {
        Carets.Add(new Caret() { Position = pageCoordinates, Selection = pageCoordinates });
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

    public void MoveCaretsSelection(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            // TODO caret move
        }
        SetCaretDirty();
    }

    public void SelectRectangle(PageCoordinates startPosition, PageCoordinates endPosition)
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


        ActionManager.DoAction(new PasteAction(this, text));
        */
    }
}