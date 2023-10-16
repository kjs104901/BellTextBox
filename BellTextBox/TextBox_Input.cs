using System.Numerics;
using Bell.Data;
using Bell.Inputs;

namespace Bell;

public partial class TextBox
{
    protected KeyboardInput KeyboardInput;
    protected MouseInput MouseInput;
    protected ViewInput ViewInput;

    public Vector2 ViewStart;
    public Vector2 ViewEnd;
    public Vector2 PageSize;
    
    private TextCoordinates _textStart;
    private TextCoordinates _textEnd;
    
    private Vector2 _mouseDragStartPage;
    private TextCoordinates _mouseDragStartText;

    protected void InputStart()
    {
        KeyboardInput.HotKeys = HotKeys.None;
        KeyboardInput.Chars.Clear();
        KeyboardInput.ImeComposition = string.Empty;

        MouseInput.Position.X = 0.0f;
        MouseInput.Position.Y = 0.0f;
        MouseInput.LeftAction = MouseAction.None;
        MouseInput.MiddleAction = MouseAction.None;
    }

    protected void InputEnd()
    {
        ProcessKeyboardHotKeys();
        ProcessKeyboardChars();
        ProcessMouseInput();
        ProcessViewInput();
    }

    private void ProcessKeyboardHotKeys()
    {
        var hk = KeyboardInput.HotKeys;

        if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Z)) // UndoAction
            UndoAction();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Y)) // RedoAction
            RedoAction();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.C)) // TODO Copy
            CopyClipboard();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.V)) // TODO Paste
            PasteClipboard();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            CopyClipboard();
            DoAction(new DeleteSelection(this));
        }
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            MoveCaretsSelection(CaretMove.StartOfFile);
            MoveCaretsPosition(CaretMove.EndOfFile);
        }
        else if (hk.HasFlag(HotKeys.Delete)) // Delete
        {
            DoAction(new DeleteSelection(this));
            DoAction(new DeleteCharAction(this, EditDirection.Forward));
        }
        else if (hk.HasFlag(HotKeys.Backspace)) // Backspace
        {
            DoAction(new DeleteSelection(this));
            DoAction(new DeleteCharAction(this, EditDirection.Backward));
        }
        else if (hk.HasFlag(HotKeys.Enter)) // Enter
        {
            DoAction(new DeleteSelection(this));
            DoAction(new EnterAction(this));
        }
        else if (hk.HasFlag(HotKeys.Tab)) // Tab
        {
            if (hk.HasFlag(HotKeys.Shift))
                DoAction(new UnTabAction(this));
            else
                DoAction(new TabAction(this));
        }
        else if (hk.HasFlag(HotKeys.UpArrow)) // Move Up
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Up);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Up);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.DownArrow)) // Move Down
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Down);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Down);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.LeftArrow)) // Move Left
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Left);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Left);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.RightArrow)) // Move Right
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Right);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Right);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.PageUp)) // Move PageUp
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.PageUp);
            }
            else
            {
                MoveCaretsPosition(CaretMove.PageUp);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.PageDown)) // Move PageDown
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.PageDown);
            }
            else
            {
                MoveCaretsPosition(CaretMove.PageDown);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.Home))
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
            }
            else
            {
                MoveCaretsPosition(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
                
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.End))
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                MoveCaretsSelection(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
            }
            else
            {
                MoveCaretsPosition(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
                
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }
    }

    private void ProcessKeyboardChars()
    {
        var hk = KeyboardInput.HotKeys;
        bool selectionDeleted = false;
        
        foreach (char keyboardInputChar in KeyboardInput.Chars)
        {
            if (keyboardInputChar == 0)
                continue;

            if (keyboardInputChar == '\n')
            {
                DoAction(new EnterAction(this));
                continue;
            }

            if (keyboardInputChar == '\t')
            {
                if (hk.HasFlag(HotKeys.Shift))
                    DoAction(new UnTabAction(this));
                else
                    DoAction(new TabAction(this));
                continue;
            }

            if (keyboardInputChar < 32)
                continue;

            if (false == selectionDeleted)
            {
                DoAction(new DeleteSelection(this));
                selectionDeleted = true;
            }
            DoAction(new InputCharAction(this, EditDirection.Forward)); // keyboardInputChar
        }
    }

    private void ProcessMouseInput()
    {
        var hk = KeyboardInput.HotKeys;

        Vector2 pageCoordinates = ViewToPage(MouseInput.Position);
        TextCoordinates textCoordinates = PageToText(pageCoordinates);

        if (textCoordinates.IsFold)
        {
            if (MouseAction.Click == MouseInput.LeftAction)
            {
                //TODO fold unfold
                // if hit then return;
            }
        }

        var mouseScreenX = MouseInput.Position.X - ViewInput.X;
        var mouseScreenY = MouseInput.Position.Y - ViewInput.Y;

        if (mouseScreenX > 0 && mouseScreenX < ViewInput.W &&
            mouseScreenY > 0 && mouseScreenY < ViewInput.H)
        {
            if (textCoordinates.IsFold)
            {
                SetMouseCursor(MouseCursor.Hand);
            }
            else if (textCoordinates.IsLineNumber)
            {
            }
            else
            {
                SetMouseCursor(MouseCursor.Beam);
            }
        }

        if (MouseAction.Click == MouseInput.LeftAction ||
            MouseAction.Click == MouseInput.MiddleAction)
        {
            _mouseDragStartPage = pageCoordinates;
            _mouseDragStartText = textCoordinates;
        }

        if (MouseAction.Click == MouseInput.LeftAction)
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                SingleCaret().Selection = textCoordinates;
            }
            else if (hk.HasFlag(HotKeys.Alt))
            {
                AddCaret(textCoordinates);
            }
            else
            {
                SingleCaret(textCoordinates);
            }
        }
        else if (MouseAction.DoubleClick == MouseInput.LeftAction)
        {
            SingleCaret(textCoordinates);

            if (hk.HasFlag(HotKeys.Shift))
            {
                // Select Line
                MoveCaretsPosition(CaretMove.EndOfLine);
                MoveCaretsSelection(CaretMove.StartOfLine);
            }
            else
            {
                // Select word
                MoveCaretsPosition(CaretMove.EndOfWord);
                MoveCaretsSelection(CaretMove.StartOfWord);
            }
        }
        else if (MouseAction.Dragging == MouseInput.LeftAction)
        {
            if (hk.HasFlag(HotKeys.Alt))
            {
                SelectRectangle(_mouseDragStartPage, pageCoordinates);
            }
            else
            {
                SingleCaret(_mouseDragStartText).Position = textCoordinates;
            }
        }
        else if (MouseAction.Dragging == MouseInput.MiddleAction)
        {
            SelectRectangle(_mouseDragStartPage, pageCoordinates);
        }
    }

    private void ProcessViewInput()
    {
        ViewStart = new Vector2()
        {
            X = ViewInput.X,
            Y = ViewInput.Y
        };
        ViewEnd = new Vector2()
        {
            X = ViewInput.X + ViewInput.W,
            Y = ViewInput.Y + ViewInput.H
        };
        
        var pageStart = ViewToPage(ViewStart);
        var pageEnd = ViewToPage(ViewEnd);

        TextCoordinates textStart = PageToText(pageStart, -3);
        TextCoordinates textEnd = PageToText(pageEnd, 3);

        if (_textStart != textStart || _textEnd != textEnd)
        {
            Text.ShowLineRendersCache.SetDirty();
            
            _textStart = textStart;
            _textEnd = textEnd;
        }
        
        if (WrapMode.Word == WrapMode || WrapMode.BreakWord == WrapMode)
        {
            PageSize.X = ViewInput.W;
        }
        PageSize.Y = Text.LineRenders.Count * GetFontSize();
    }
}