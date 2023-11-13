using System.Text;
using Bell.Actions;
using Bell.Utils;

namespace Bell.Data;

// Interface
internal partial class CaretManager
{
    internal static void ClearCarets() => Singleton.TextBox.CaretManager.ClearCarets_();
    internal static void AddCaret(Coordinates coordinates) => Singleton.TextBox.CaretManager.AddCaret_(coordinates);
    internal static void AddCaret(Caret caret) => Singleton.TextBox.CaretManager.AddCaret_(caret);
    internal static int Count => Singleton.TextBox.CaretManager._carets.Count;
    internal static Caret GetCaret(int index) => Singleton.TextBox.CaretManager._carets[index];
    internal static bool GetFirstCaret(out Caret caret) => Singleton.TextBox.CaretManager.GetFirstCaret_(out caret);

    internal static void MoveCaretsPosition(CaretMove caretMove) =>
        Singleton.TextBox.CaretManager.MoveCaretsPosition_(caretMove);

    internal static void MoveCaretsAnchor(CaretMove caretMove) =>
        Singleton.TextBox.CaretManager.MoveCaretsAnchor_(caretMove);

    internal static bool HasCaretsSelection() => Singleton.TextBox.CaretManager.HasCaretsSelection_();
    internal static void RemoveCaretsSelection() => Singleton.TextBox.CaretManager.RemoveCaretsSelection_();
    internal static void RemoveCaretsLineSub() => Singleton.TextBox.CaretManager.RemoveCaretsLineSub_();

    internal static void SelectRectangle(Coordinates startPosition, Coordinates endPosition) =>
        Singleton.TextBox.CaretManager.SelectRectangle_(startPosition, endPosition);

    internal static List<Caret> GetCaretsInLine(Line line) => Singleton.TextBox.CaretManager.GetCaretsInLine_(line);
    internal static void CopyClipboard() => Singleton.TextBox.CaretManager.CopyClipboard_();
    internal static void PasteClipboard() => Singleton.TextBox.CaretManager.PasteClipboard_();
    internal static bool CheckValid(Caret caret) => Singleton.TextBox.CaretManager.CheckValid_(caret);
    internal static string GetDebugString() => Singleton.TextBox.CaretManager.GetDebugString_();
    
    internal static void ShiftCaretChar(Caret caret, EditDirection direction, int count, bool isUndo) =>
        Singleton.TextBox.CaretManager.ShiftCaretChar_(caret, direction, count, isUndo);
    internal static void ShiftCaretLine(int lineIndex, EditDirection direction) =>
        Singleton.TextBox.CaretManager.ShiftCaretLine_(lineIndex, direction);
    
    internal static void MergeLineCaret(Line line, Line fromLine, bool isUndo) =>
        Singleton.TextBox.CaretManager.MergeLineCaret_(line, fromLine, isUndo);
    internal static void SplitLineCaret(Caret caret, Line line, Line toLine, bool isUndo) =>
        Singleton.TextBox.CaretManager.SplitLineCaret_(caret, line, toLine, isUndo);
}

// Implementation
internal partial class CaretManager
{
    private readonly List<Caret> _carets = new();
    private IEnumerable<Caret> ReversedCarets()
    {
        for (int i = _carets.Count-1; i >= 0; i--)
        {
            yield return _carets[i];
        }
    }

    private IEnumerable<Caret> Carets()
    {
        for (int i = 0; i < _carets.Count; i++)
        {
            yield return _carets[i];
        }
    }

    private void ClearCarets_()
    {
        _carets.Clear();
    }

    private void AddCaret_(Coordinates coordinates)
    {
        AddCaret_(new Caret() { Position = coordinates, AnchorPosition = coordinates });
    }

    private void AddCaret_(Caret newCaret)
    {
        if (false == CheckValid_(newCaret))
        {
            Logger.Error(
                $"AddCaret: invalid caret: {newCaret.Position.LineIndex} {newCaret.Position.CharIndex} {newCaret.AnchorPosition.LineIndex} {newCaret.AnchorPosition.CharIndex}");
            return;
        }
        
        newCaret.GetSorted(out Coordinates newStart, out Coordinates newEnd);
        foreach (Caret caret in _carets)
        {
            caret.GetSorted(out Coordinates start, out Coordinates end);

            if (start.IsBiggerThanWithoutLineSub(newEnd) || newStart.IsBiggerThanWithoutLineSub(end))
                continue;
            
            Logger.Info("AddCaret: already exist caret");
            return;
        }
        _carets.Add(newCaret);
    }

    private bool GetFirstCaret_(out Caret caret)
    {
        if (_carets.Count > 1)
        {
            _carets.RemoveRange(1, _carets.Count - 1);
        }

        if (_carets.Count > 0)
        {
            caret = _carets[0];
            return true;
        }

        caret = Caret.None;
        return false;
    }

    private void MoveCaretsPosition_(CaretMove caretMove)
    {
        foreach (Caret caret in _carets)
        {
            caret.Position = caret.Position.FindMove(caretMove);
        }
        RowManager.SetRowCacheDirty();
    }

    private void MoveCaretsAnchor_(CaretMove caretMove)
    {
        foreach (Caret caret in _carets)
        {
            caret.AnchorPosition = caret.AnchorPosition.FindMove(caretMove);
        }
        RowManager.SetRowCacheDirty();
    }

    private bool HasCaretsSelection_()
    {
        foreach (Caret caret in _carets)
        {
            if (caret.HasSelection)
                return true;
        }
        return false;
    }

    private void RemoveCaretsSelection_()
    {
        foreach (Caret caret in _carets)
        {
            caret.AnchorPosition = caret.Position;
        }
        RowManager.SetRowCacheDirty();
    }

    private void RemoveCaretsLineSub_()
    {
        foreach (Caret caret in _carets)
        {
            caret.Position.LineSubIndex = -1;
            caret.AnchorPosition.LineSubIndex = -1;
        }
        RowManager.SetRowCacheDirty();
    }

    private void SelectRectangle_(Coordinates startPosition, Coordinates endPosition)
    {
        _carets.Clear();
        // TODO select multiple lines
        RowManager.SetRowCacheDirty();
    }

    private List<Caret> GetCaretsInLine_(Line line)
    {
        return _carets.Where(caret => caret.Position.LineIndex == line.Index).ToList();
    }

    private void CopyClipboard_()
    {
        /*
        if (CaretManager._carets.Count == 0)
            return;

        StringBuilder.Clear();
        foreach (Caret caret in CaretManager._carets)
        {
            if (caret.HasSelection)
            {
                StringBuilder.Append(Text.GetText(caret.SelectionStart, caret.SelectionEnd));
            }
        }

        SetClipboard(StringBuilder.ToString());
        */
    }

    private void PasteClipboard_()
    {
        /*
        if (CaretManager._carets.Count == 0)
            return;

        string text = GetClipboard();
        if (text == null)
            return;

        // if caret number is same as text line number, paste to each line


        ActionManager.DoAction(new PasteAction(text));
        */
    }

    private bool CheckValid_(Caret caret)
    {
        if (false == caret.Position.IsValid())
            return false;

        if (false == caret.AnchorPosition.IsValid())
            return false;

        return true;
    }

    private string GetDebugString_()
    {
        StringBuilder sb = new();
        foreach (Caret caret in _carets)
        {
            sb.AppendLine("Caret:");
            sb.AppendLine("\tPosition\t" + caret.Position.LineIndex + ":" + caret.Position.CharIndex + ":" +
                          caret.Position.LineSubIndex);
            sb.AppendLine("\tAnchorPosition\t" + caret.AnchorPosition.LineIndex + ":" + caret.AnchorPosition.CharIndex +
                          ":" + caret.AnchorPosition.LineSubIndex);
        }

        return sb.ToString();
    }

    private void ShiftCaretChar_(Caret caret, EditDirection direction, int count, bool isUndo)
    {
        int moveCount = count * (EditDirection.Forward == direction ? 1 : -1);

        foreach (Caret c in isUndo ? ReversedCarets() : Carets())
        {
            if (c.Position.LineIndex == caret.Position.LineIndex)
            {
                if (caret.Position.CharIndex <= c.Position.CharIndex)
                {
                    c.Position.CharIndex += moveCount;
                    
                    if (c.Position.CharIndex < 0)
                    {
                        c.Position.CharIndex = 0;
                    }
                }
            }

            if (c.AnchorPosition.LineIndex == caret.Position.LineIndex)
            {
                if (caret.Position.CharIndex <= c.AnchorPosition.CharIndex)
                {
                    c.AnchorPosition.CharIndex += moveCount;
                    
                    if (c.AnchorPosition.CharIndex < 0)
                    {
                        c.AnchorPosition.CharIndex = 0;
                    }
                }
            }
            
            Logger.Info("ShiftCaretChar: " + c.Position.LineIndex + " " + c.Position.CharIndex + " " + moveCount);
        }
    }

    private void ShiftCaretLine_(int lineIndex, EditDirection direction)
    {
        int moveCount = EditDirection.Forward == direction ? 1 : -1;
        foreach (Caret c in Carets())
        {
            if (lineIndex < c.Position.LineIndex)
            {
                c.Position.LineIndex += moveCount;
            }

            if (lineIndex < c.AnchorPosition.LineIndex)
            {
                c.AnchorPosition.LineIndex += moveCount;
            }
            
            Logger.Info("ShiftCaretLine: " + c.Position.LineIndex + " " + c.Position.CharIndex + " " + moveCount);
        }
    }

    private void MergeLineCaret_(Line line, Line fromLine, bool isUndo)
    {
        foreach (Caret c in isUndo ? ReversedCarets() : Carets())
        {
            if (c.Position.LineIndex == fromLine.Index)
            {
                c.Position.LineIndex = line.Index;
                c.Position.CharIndex += line.Chars.Count;
            }

            if (c.AnchorPosition.LineIndex == fromLine.Index)
            {
                c.AnchorPosition.LineIndex = line.Index;
                c.AnchorPosition.CharIndex += line.Chars.Count;
            }
            
            Logger.Info("MergeLineCaret: " + c.Position.LineIndex + " " + c.Position.CharIndex);
        }
    }

    private void SplitLineCaret_(Caret caret, Line line, Line toLine, bool isUndo)
    {
        foreach (Caret c in isUndo ? ReversedCarets() : Carets())
        {
            if (c.Position.LineIndex == caret.Position.LineIndex)
            {
                if (caret.Position.CharIndex <= c.Position.CharIndex)
                {
                    c.Position.LineIndex = toLine.Index;
                    c.Position.CharIndex -= line.Chars.Count;
                    
                    if (c.Position.CharIndex < 0)
                    {
                        c.Position.CharIndex = 0;
                    }
                }
            }

            if (c.AnchorPosition.LineIndex == caret.Position.LineIndex)
            {
                if (caret.Position.CharIndex <= c.AnchorPosition.CharIndex)
                {
                    c.AnchorPosition.LineIndex = toLine.Index;
                    c.AnchorPosition.CharIndex -= line.Chars.Count;
                    
                    if (c.AnchorPosition.CharIndex < 0)
                    {
                        c.AnchorPosition.CharIndex = 0;
                    }
                }
            }
            
            Logger.Info("SplitLineCaret: " + c.Position.LineIndex + " " + c.Position.CharIndex);
        }
    }
}
    