﻿using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    private Vector2 _viewPos;
    private Vector2 _viewSize;

    public Vector2 PageSize;

    private TextCoordinates _textStart;
    private TextCoordinates _textEnd;

    private Vector2 _mouseDragStartPage;
    private TextCoordinates _mouseDragStartText;

    private bool _shiftPressed;
    private bool _altPressed;

    private string _imeComposition = "";

    private void ProcessInput()
    {
        ProcessKeyboardInput();
        ProcessMouseInput();
        ProcessViewInput();

        _backend.OnInputEnd();
    }

    private void ProcessKeyboardInput()
    {
        _caretChanged = false;

        KeyboardInput keyboardInput = _backend.GetKeyboardInput();

        var hk = keyboardInput.HotKeys;

        _shiftPressed |= EnumFlag.Has(hk, HotKeys.Shift);
        _altPressed |= EnumFlag.Has(hk, HotKeys.Alt);
        _imeComposition = keyboardInput.ImeComposition;

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

    private void ProcessMouseInput()
    {
        MouseInput mouseInput = _backend.GetMouseInput();

        Vector2 pageCoordinates = ViewToPage(mouseInput.Position);
        TextCoordinates textCoordinates = PageToText(pageCoordinates);

        if (textCoordinates.IsFold)
        {
            if (MouseAction.Click == mouseInput.LeftAction)
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

    protected void ProcessViewInput()
    {
        var pageStart = ViewToPage(_viewPos);
        var pageEnd = ViewToPage(_viewPos + _viewSize);

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
            PageSize.X = _viewSize.X;
        }

        PageSize.Y = SubLines.Count * GetFontHeight();
    }
}