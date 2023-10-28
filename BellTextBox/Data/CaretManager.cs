using System.Text;
using Bell.Utils;

namespace Bell.Data;

public class CaretManager
{
    private readonly List<Caret> _carets = new();

    internal void ClearCarets()
    {
        _carets.Clear();
        Singleton.RowManager.OnRowChanged();
    }

    internal void AddCaret(Coordinates coordinates)
    {
        AddCaret(new Caret() { Position = coordinates, AnchorPosition = coordinates });
    }
    
    internal void AddCaret(Caret caret)
    {
        if (false == CheckValid(caret))
        {
            Logger.Error(
                $"AddCaret: invalid caret: {caret.Position.LineIndex} {caret.Position.CharIndex} {caret.AnchorPosition.LineIndex} {caret.AnchorPosition.CharIndex}");
            return;
        }
        _carets.Add(caret);
        Singleton.RowManager.OnRowChanged();
    }

    internal int Count => _carets.Count;
    internal Caret GetCaret(int index) => _carets[index];

    internal Caret SingleCaret(Coordinates coordinates = new())
    {
        if (_carets.Count > 1)
            _carets.RemoveRange(1, _carets.Count - 1);

        if (_carets.Count > 0)
        {
            _carets[0].Position = coordinates;
            _carets[0].AnchorPosition = coordinates;
        }
        else
        {
            _carets.Add(new Caret() { Position = coordinates, AnchorPosition = coordinates });
        }

        Singleton.RowManager.OnRowChanged();
        return _carets[0];
    }

    public void MoveCaretsPosition(CaretMove caretMove)
    {
        foreach (Caret caret in _carets)
        {
            caret.Position = caret.Position.FindMove(caretMove);
        }
        Singleton.RowManager.OnRowChanged();
    }

    public void MoveCaretsAnchor(CaretMove caretMove)
    {
        foreach (Caret caret in _carets)
        {
            caret.AnchorPosition = caret.AnchorPosition.FindMove(caretMove);
        }
        Singleton.RowManager.OnRowChanged();
    }


    internal bool HasCaretsSelection()
    {
        foreach (Caret caret in _carets)
        {
            if (caret.HasSelection)
                return true;
        }

        return false;
    }

    public void RemoveCaretsSelection()
    {
        foreach (Caret caret in _carets)
        {
            caret.AnchorPosition = caret.Position;
        }

        Singleton.RowManager.OnRowChanged();
    }

    internal void SelectRectangle(Coordinates startPosition, Coordinates endPosition)
    {
        _carets.Clear();
        // TODO select multiple lines
        Singleton.RowManager.OnRowChanged();
    }

    internal List<Caret> GetCaretsInLine(Line line)
    {
        return _carets.Where(caret => caret.Position.LineIndex == line.Index).ToList();
    }

    internal void CopyClipboard()
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

    internal void PasteClipboard()
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

    public bool CheckValid(Caret caret)
    {
        if (false == caret.Position.IsValid())  
            return false;
        
        if (false == caret.AnchorPosition.IsValid())  
            return false;
        
        return true;
    }

    public string GetDebugString()
    {
        StringBuilder sb = new();
        foreach (Caret caret in _carets)
        {
            sb.AppendLine("Caret:");
            sb.AppendLine("\tPosition\t" + caret.Position.LineIndex + ":" + caret.Position.CharIndex);
            sb.AppendLine("\tAnchorPosition\t" + caret.AnchorPosition.LineIndex + ":" + caret.AnchorPosition.CharIndex);
        }

        return sb.ToString();
    }
}