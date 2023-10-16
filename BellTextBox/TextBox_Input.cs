using System.Numerics;
using Bell.Data;
using Bell.Inputs;

namespace Bell;

public partial class TextBox
{
    //protected KeyboardInput KeyboardInput;
    //protected MouseInput MouseInput;
    //protected ViewInput ViewInput;

    public Vector2 ViewStart;
    public Vector2 ViewEnd;
    public Vector2 PageSize;

    private bool _shiftPressed;
    private bool _altPressed;
    
    private string _imeComposition;
    
    private TextCoordinates _textStart;
    private TextCoordinates _textEnd;
    
    private Vector2 _mouseDragStartPage;
    private TextCoordinates _mouseDragStartText;

    protected void InputStart()
    {
        //KeyboardInput.HotKeys = HotKeys.None;
        //KeyboardInput.Chars.Clear();
        //KeyboardInput.ImeComposition = string.Empty;
//
        //MouseInput.Position.X = 0.0f;
        //MouseInput.Position.Y = 0.0f;
        //MouseInput.LeftAction = MouseAction.None;
        //MouseInput.MiddleAction = MouseAction.None;
    }

    protected void ProcessKeyboardInput(KeyboardInput keyboardInput)
    {
        var hk = keyboardInput.HotKeys;

        _shiftPressed |= hk.HasFlag(HotKeys.Shift);
        _altPressed |= hk.HasFlag(HotKeys.Alt);
        _imeComposition = keyboardInput.ImeComposition;

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
        
        // Chars
        bool selectionDeleted = false;
        if (null != keyboardInput.Chars)
        {
            foreach (char keyboardInputChar in keyboardInput.Chars)
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
    }

    protected void ProcessMouseInput(MouseInput mouseInput)
    {
        Vector2 pageCoordinates = ViewToPage(mouseInput.Position);
        TextCoordinates textCoordinates = PageToText(pageCoordinates);

        if (textCoordinates.IsFold)
        {
            if (MouseAction.Click == mouseInput.LeftAction)
            {
                //TODO fold unfold
                // if hit then return;
            }
        }

        if (mouseInput.Position.X > ViewStart.X && mouseInput.Position.X < ViewEnd.X &&
            mouseInput.Position.Y > ViewStart.Y && mouseInput.Position.Y < ViewEnd.Y)
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

        if (MouseAction.Click == mouseInput.LeftAction ||
            MouseAction.Click == mouseInput.MiddleAction)
        {
            _mouseDragStartPage = pageCoordinates;
            _mouseDragStartText = textCoordinates;
        }

        if (MouseAction.Click == mouseInput.LeftAction)
        {
            if (_shiftPressed)
            {
                SingleCaret().Selection = textCoordinates;
            }
            else if (_altPressed)
            {
                AddCaret(textCoordinates);
            }
            else
            {
                SingleCaret(textCoordinates);
            }
        }
        else if (MouseAction.DoubleClick == mouseInput.LeftAction)
        {
            SingleCaret(textCoordinates);

            if (_shiftPressed)
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
        else if (MouseAction.Dragging == mouseInput.LeftAction)
        {
            if (_altPressed)
            {
                SelectRectangle(_mouseDragStartPage, pageCoordinates);
            }
            else
            {
                SingleCaret(_mouseDragStartText).Position = textCoordinates;
            }
        }
        else if (MouseAction.Dragging == mouseInput.MiddleAction)
        {
            SelectRectangle(_mouseDragStartPage, pageCoordinates);
        }

        _shiftPressed = false;
        _altPressed = false;
    }

    protected void ProcessViewInput(ViewInput viewInput)
    {
        ViewStart = new Vector2()
        {
            X = viewInput.X,
            Y = viewInput.Y
        };
        ViewEnd = new Vector2()
        {
            X = viewInput.X + viewInput.W,
            Y = viewInput.Y + viewInput.H
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
            PageSize.X = viewInput.W;
        }
        PageSize.Y = Text.LineRenders.Count * GetFontHeight();
    }
}