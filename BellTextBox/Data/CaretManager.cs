using Bell.Utils;

namespace Bell.Data;

public class CaretManager
{
    public readonly List<Caret> Carets = new();

    internal void SetCaretDirty()
    {
        foreach (SubLine row in ThreadLocal.LineManager.Rows)
        {
            row.LineSelectionCache.SetDirty();
        }
    }

    internal Caret SingleCaret(LineCoordinates lineCoordinates = new())
    {
        if (Carets.Count > 1)
            Carets.RemoveRange(1, Carets.Count - 1);

        if (Carets.Count > 0)
        {
            Carets[0].Position = lineCoordinates;
            Carets[0].AnchorPosition = lineCoordinates;
        }
        else
        {
            Carets.Add(new Caret() { Position = lineCoordinates, AnchorPosition = lineCoordinates });
        }

        SetCaretDirty();
        return Carets[0];
    }

    internal void AddCaret(LineCoordinates lineCoordinates)
    {
        Carets.Add(new Caret() { Position = lineCoordinates, AnchorPosition = lineCoordinates });
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
            caret.Position = caret.Position.FindCoordinates(caretMove);
        }
        SetCaretDirty();
    }
    
    public void MoveCaretsAnchor(CaretMove caretMove)
    {
        foreach (Caret caret in Carets)
        {
            caret.AnchorPosition = caret.AnchorPosition.FindCoordinates(caretMove);
        }
        SetCaretDirty();
    }
    

    internal bool HasCaretsSelection()
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

    internal void SelectRectangle(LineCoordinates startPosition, LineCoordinates endPosition)
    {
        Carets.Clear();
        // TODO select multiple lines
        SetCaretDirty();
    }

    internal void CopyClipboard()
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

    internal void PasteClipboard()
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