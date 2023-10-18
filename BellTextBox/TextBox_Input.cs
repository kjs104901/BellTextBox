using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    //protected KeyboardInput KeyboardInput;
    //protected MouseInput MouseInput;
    //protected ViewInput ViewInput;

    public Vector2 ViewStart;
    public Vector2 ViewEnd;
    public Vector2 PageSize;

    private string _imeComposition;

    private TextCoordinates _textStart;
    private TextCoordinates _textEnd;

    private Vector2 _mouseDragStartPage;
    private TextCoordinates _mouseDragStartText;

    protected KeyboardInput KeyboardInput = new KeyboardInput() { Chars = new List<char>() };
    protected MouseInput MouseInput = new MouseInput();
    protected ViewInput ViewInput = new ViewInput();
    private bool _shiftPressed;
    private bool _altPressed;

    protected void ClearKeyboardInput()
    {
        KeyboardInput.HotKeys = HotKeys.None;
        KeyboardInput.Chars?.Clear();
        KeyboardInput.ImeComposition = string.Empty;
    }

    protected void ClearMouseInput()
    {
        MouseInput.Position.X = 0.0f;
        MouseInput.Position.Y = 0.0f;
        MouseInput.LeftAction = MouseAction.None;
        MouseInput.MiddleAction = MouseAction.None;
    }

    protected void ClearViewInput()
    {
        ViewInput.X = 0.0f;
        ViewInput.Y = 0.0f;
        ViewInput.W = 0.0f;
        ViewInput.H = 0.0f;
    }

    protected void ProcessKeyboardInput()
    {
        _caretChanged = false;

        var hk = KeyboardInput.HotKeys;

        _shiftPressed |= EnumFlag.Has(hk, HotKeys.Shift);
        _altPressed |= EnumFlag.Has(hk, HotKeys.Alt);
        _imeComposition = KeyboardInput.ImeComposition;

        if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Z)) // UndoAction
            UndoAction();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Y)) // RedoAction
            RedoAction();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.C)) // TODO Copy
            CopyClipboard();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.V)) // TODO Paste
            PasteClipboard();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            CopyClipboard();
            DoAction(new DeleteSelection(this));
        }
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            MoveCaretsSelection(CaretMove.StartOfFile);
            MoveCaretsPosition(CaretMove.EndOfFile);
        }
        else if (EnumFlag.Has(hk, HotKeys.Delete)) // Delete
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.StartOfFile);
                MoveCaretsPosition(CaretMove.EndOfFile);
                DoAction(new DeleteSelection(this));
            }
            else
            {
                DoAction(new DeleteSelection(this));
                DoAction(new DeleteCharAction(this, EditDirection.Forward));
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Backspace)) // Backspace
        {
            if (EnumFlag.Has(hk, HotKeys.Alt))
            {
                UndoAction();
            }
            else
            {
                DoAction(new DeleteSelection(this));
                DoAction(new DeleteCharAction(this, EditDirection.Backward));
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Enter)) // Enter
        {
            DoAction(new DeleteSelection(this));
            DoAction(new EnterAction(this));
        }
        else if (EnumFlag.Has(hk, HotKeys.Tab)) // Tab
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
                DoAction(new UnTabAction(this));
            else
                DoAction(new TabAction(this));
        }
        else if (EnumFlag.Has(hk, HotKeys.UpArrow)) // Move Up
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Up);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Up);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.DownArrow)) // Move Down
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Down);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Down);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.LeftArrow)) // Move Left
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Left);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Left);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.RightArrow)) // Move Right
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.Right);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Right);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.PageUp)) // Move PageUp
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.PageUp);
            }
            else
            {
                MoveCaretsPosition(CaretMove.PageUp);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.PageDown)) // Move PageDown
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(CaretMove.PageDown);
            }
            else
            {
                MoveCaretsPosition(CaretMove.PageDown);
                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Home))
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
            }
            else
            {
                MoveCaretsPosition(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);

                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.End))
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsSelection(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
            }
            else
            {
                MoveCaretsPosition(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);

                MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }

        // Chars
        bool selectionDeleted = false;
        if (null != KeyboardInput.Chars)
        {
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
                    if (EnumFlag.Has(hk, HotKeys.Shift))
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

    protected void ProcessMouseInput()
    {
        Vector2 pageCoordinates = ViewToPage(MouseInput.Position);
        TextCoordinates textCoordinates = PageToText(pageCoordinates);

        if (textCoordinates.IsFold)
        {
            if (MouseAction.Click == MouseInput.LeftAction)
            {
                var folding = SubLines[textCoordinates.Row].Folding;
                if (null != folding)
                {
                    folding.Folded = !folding.Folded;
                    SingleCaret(textCoordinates);

                    SubLinesCache.SetDirty();
                    VisibleSubLinesCache.SetDirty();
                    return;
                }
            }
        }

        if (MouseInput.Position.X > ViewStart.X && MouseInput.Position.X < ViewEnd.X &&
            MouseInput.Position.Y > ViewStart.Y && MouseInput.Position.Y < ViewEnd.Y)
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
        else if (MouseAction.DoubleClick == MouseInput.LeftAction)
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
        else if (MouseAction.Dragging == MouseInput.LeftAction)
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
        else if (MouseAction.Dragging == MouseInput.MiddleAction)
        {
            SelectRectangle(_mouseDragStartPage, pageCoordinates);
        }

        _shiftPressed = false;
        _altPressed = false;
    }

    protected void ProcessViewInput()
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
            VisibleSubLinesCache.SetDirty();

            _textStart = textStart;
            _textEnd = textEnd;
        }

        if (WrapMode.Word == WrapMode || WrapMode.BreakWord == WrapMode)
        {
            PageSize.X = ViewInput.W;
        }

        PageSize.Y = SubLines.Count * GetFontHeight();
    }
}