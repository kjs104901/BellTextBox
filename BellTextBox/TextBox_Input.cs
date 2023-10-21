using System.Numerics;
using Bell.Actions;
using Bell.Data;
using Bell.Inputs;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    private Vector2 _viewPos;
    private Vector2 _viewSize;

    private PageCoordinates _pageStart;
    private PageCoordinates _pageEnd;

    private TextCoordinates _textStart;
    private TextCoordinates _textEnd;

    public Vector2 PageSize;

    private TextCoordinates _mouseDragStartText;

    private bool _shiftPressed;
    private bool _altPressed;

    private string _imeComposition = "";

    private void ProcessInput(Vector2 viewPos, Vector2 viewSize)
    {
        ProcessKeyboardInput();
        ProcessMouseInput();
        ProcessViewInput(viewPos, viewSize);
        _backend.OnInputEnd();
    }

    private void ProcessKeyboardInput()
    {
        KeyboardInput keyboardInput = _backend.GetKeyboardInput();

        var hk = keyboardInput.HotKeys;

        _shiftPressed |= EnumFlag.Has(hk, HotKeys.Shift);
        _altPressed |= EnumFlag.Has(hk, HotKeys.Alt);
        _imeComposition = keyboardInput.ImeComposition;

        if (false == string.IsNullOrEmpty(_imeComposition))
        {
            if (HasCaretsSelection())
            {
                DoAction(new DeleteSelection());
                RemoveCaretsSelection();
            }
        }

        if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Shift | HotKeys.Z)) // RedoAction
            RedoAction();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Z)) // UndoAction
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
            DoAction(new DeleteSelection());
            RemoveCaretsSelection();
        }
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            MoveCaretsAnchor(CaretMove.StartOfFile);
            MoveCaretsPosition(CaretMove.EndOfFile);
        }
        else if (EnumFlag.Has(hk, HotKeys.Delete)) // Delete
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.StartOfFile);
                MoveCaretsPosition(CaretMove.EndOfFile);
                DoAction(new DeleteSelection());
                RemoveCaretsSelection();
            }
            else
            {
                if (HasCaretsSelection())
                {
                    DoAction(new DeleteSelection());
                    RemoveCaretsSelection();
                }
                else
                {
                    DoAction(new DeleteCharAction(EditDirection.Forward));
                }
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
                if (HasCaretsSelection())
                {
                    DoAction(new DeleteSelection());
                    RemoveCaretsSelection();
                }
                else
                {
                    DoAction(new DeleteCharAction(EditDirection.Backward));
                }
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Enter)) // Enter
        {
            DoAction(new DeleteSelection());
            RemoveCaretsSelection();
            DoAction(new EnterAction());
        }
        else if (EnumFlag.Has(hk, HotKeys.Tab)) // Tab
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
                DoAction(new UnTabAction());
            else
                DoAction(new TabAction());
        }
        else if (EnumFlag.Has(hk, HotKeys.UpArrow)) // Move Up
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.Up);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Up);
                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.DownArrow)) // Move Down
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.Down);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Down);
                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.LeftArrow)) // Move Left
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.Left);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Left);
                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.RightArrow)) // Move Right
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.Right);
            }
            else
            {
                MoveCaretsPosition(CaretMove.Right);
                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.PageUp)) // Move PageUp
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.PageUp);
            }
            else
            {
                MoveCaretsPosition(CaretMove.PageUp);
                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.PageDown)) // Move PageDown
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(CaretMove.PageDown);
            }
            else
            {
                MoveCaretsPosition(CaretMove.PageDown);
                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Home))
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
            }
            else
            {
                MoveCaretsPosition(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);

                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.End))
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                MoveCaretsAnchor(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
            }
            else
            {
                MoveCaretsPosition(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);

                RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }

        // Chars
        foreach (char keyboardInputChar in keyboardInput.Chars)
        {
            if (keyboardInputChar == 0)
                continue;

            if (keyboardInputChar == '\n')
            {
                DoAction(new EnterAction());
                continue;
            }

            if (keyboardInputChar == '\t')
            {
                if (EnumFlag.Has(hk, HotKeys.Shift))
                    DoAction(new UnTabAction());
                else
                    DoAction(new TabAction());
                continue;
            }

            if (keyboardInputChar < 32)
                continue;

            if (HasCaretsSelection())
            {
                DoAction(new DeleteSelection());
                RemoveCaretsSelection();
            }

            DoAction(new InputCharAction(EditDirection.Forward, keyboardInputChar));
        }
    }

    private void ProcessMouseInput()
    {
        MouseInput mouseInput = _backend.GetMouseInput();

        ConvertCoordinates(mouseInput.Position,
            out PageCoordinates pageCoordinates,
            out TextCoordinates textCoordinates);

        if (textCoordinates.IsFold)
        {
            if (MouseAction.Click == mouseInput.LeftAction)
            {
                if (GetLine(textCoordinates.LineIndex, out Line? line))
                {
                    if (null != line?.Folding)
                    {
                        line.Folding.Folded = !line.Folding.Folded;

                        RowsCache.SetDirty();
                        SetCaretDirty();

                        return;
                    }
                }
            }
        }

        if (mouseInput.Position.X > _viewPos.X && mouseInput.Position.X < _viewPos.X + _viewSize.X &&
            mouseInput.Position.Y > _viewPos.Y && mouseInput.Position.Y < _viewPos.Y + _viewSize.Y)
        {
            if (textCoordinates.IsFold)
            {
                _backend.SetMouseCursor(MouseCursor.Hand);
            }
            else if (textCoordinates.IsLineNumber)
            {
            }
            else
            {
                _backend.SetMouseCursor(MouseCursor.Beam);
            }
        }

        if (MouseAction.Click == mouseInput.LeftAction ||
            MouseAction.Click == mouseInput.MiddleAction)
        {
            _mouseDragStartText = textCoordinates;
        }

        if (MouseAction.Click == mouseInput.LeftAction)
        {
            if (_shiftPressed)
            {
                SingleCaret().AnchorPosition = textCoordinates;
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
                MoveCaretsAnchor(CaretMove.StartOfLine);
            }
            else
            {
                // Select word
                MoveCaretsPosition(CaretMove.EndOfWord);
                MoveCaretsAnchor(CaretMove.StartOfWord);
            }
        }
        else if (MouseAction.Dragging == mouseInput.LeftAction)
        {
            if (_altPressed)
            {
                SelectRectangle(_mouseDragStartText, textCoordinates);
            }
            else
            {
                SingleCaret(_mouseDragStartText).Position = textCoordinates;
            }
        }
        else if (MouseAction.Dragging == mouseInput.MiddleAction)
        {
            SelectRectangle(_mouseDragStartText, textCoordinates);
        }

        _shiftPressed = false;
        _altPressed = false;
    }

    protected void ProcessViewInput(Vector2 viewPos, Vector2 viewSize)
    {
        if (MathHelper.IsNotSame(viewPos.X, _viewPos.X) ||
            MathHelper.IsNotSame(viewPos.Y, _viewPos.Y) ||
            MathHelper.IsNotSame(viewSize.X, _viewSize.X) ||
            MathHelper.IsNotSame(viewSize.Y, _viewSize.Y))
        {
            _viewPos = viewPos;
            _viewSize = viewSize;

            ConvertCoordinates(_viewPos, out _pageStart, out _textStart, -3);
            ConvertCoordinates(_viewPos + _viewSize, out _pageEnd, out _textEnd, 3);

            foreach (Line line in Lines)
            {
                line.CutoffsCache.SetDirty();
                line.SubLinesCache.SetDirty();
            }

            RowsCache.SetDirty();
        }

        if (WrapMode.Word == WrapMode || WrapMode.BreakWord == WrapMode)
        {
            PageSize.X = _viewSize.X;
        }

        PageSize.Y = Rows.Count * GetFontHeight();
    }
}