﻿using System.Numerics;
using Bell.Actions;
using Bell.Data;
using Bell.Inputs;
using Bell.Utils;

namespace Bell;

public partial class TextBox
{
    private Vector2 _viewPos;
    private Vector2 _viewSize;

    public Vector2 PageSize;

    private Coordinates _mouseDragStartText;

    private bool _shiftPressed;
    private bool _altPressed;

    private string _imeComposition = "";

    private void ProcessInput(Vector2 viewPos, Vector2 viewSize)
    {
        ProcessMouseInput();
        ProcessKeyboardInput();
        ProcessViewInput(viewPos, viewSize);
        Backend.OnInputEnd();
    }

    private void ProcessKeyboardInput()
    {
        KeyboardInput keyboardInput = Backend.GetKeyboardInput();

        var hk = keyboardInput.HotKeys;

        _shiftPressed |= EnumFlag.Has(hk, HotKeys.Shift);
        _altPressed |= EnumFlag.Has(hk, HotKeys.Alt);
        
        // Chars
        foreach (char keyboardInputChar in keyboardInput.Chars)
        {
            if (keyboardInputChar == 0)
                continue;

            if (keyboardInputChar == '\n')
            {
                ActionManager.DoAction(new EnterAction());
                continue;
            }

            if (keyboardInputChar == '\t')
            {
                if (EnumFlag.Has(hk, HotKeys.Shift))
                    ActionManager.DoAction(new UnTabAction());
                else
                    ActionManager.DoAction(new TabAction());
                continue;
            }

            if (keyboardInputChar < 32)
                continue;

            if (CaretManager.HasCaretsSelection())
            {
                ActionManager.DoAction(new DeleteSelection());
                CaretManager.RemoveCaretsSelection();
            }

            ActionManager.DoAction(new InputCharAction(EditDirection.Forward, keyboardInputChar));
        }

        // IME
        if (false == string.IsNullOrEmpty(keyboardInput.ImeComposition))
        {
            if (CaretManager.HasCaretsSelection())
            {
                ActionManager.DoAction(new DeleteSelection());
                CaretManager.RemoveCaretsSelection();
            }
        }

        if (_imeComposition != keyboardInput.ImeComposition)
        {
            _imeComposition = keyboardInput.ImeComposition;
            return;
        }

        if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Shift | HotKeys.Z)) // RedoAction
            ActionManager.RedoAction();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Z)) // UndoAction
            ActionManager.UndoAction();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.Y)) // RedoAction
            ActionManager.RedoAction();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.C)) // TODO Copy
            CaretManager.CopyClipboard();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.V)) // TODO Paste
            CaretManager.PasteClipboard();
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            CaretManager.CopyClipboard();
            ActionManager.DoAction(new DeleteSelection());
            CaretManager.RemoveCaretsSelection();
        }
        else if (EnumFlag.Has(hk, HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            CaretManager.MoveCaretsAnchor(CaretMove.StartOfFile);
            CaretManager.MoveCaretsPosition(CaretMove.EndOfFile);
        }
        else if (EnumFlag.Has(hk, HotKeys.Delete)) // Delete
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.StartOfFile);
                CaretManager.MoveCaretsPosition(CaretMove.EndOfFile);
                ActionManager.DoAction(new DeleteSelection());
                CaretManager.RemoveCaretsSelection();
            }
            else
            {
                if (CaretManager.HasCaretsSelection())
                {
                    ActionManager.DoAction(new DeleteSelection());
                    CaretManager.RemoveCaretsSelection();
                }
                else
                {
                    ActionManager.DoAction(new DeleteCharAction(EditDirection.Forward));
                }
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Backspace)) // Backspace
        {
            if (EnumFlag.Has(hk, HotKeys.Alt))
            {
                ActionManager.UndoAction();
            }
            else
            {
                if (CaretManager.HasCaretsSelection())
                {
                    ActionManager.DoAction(new DeleteSelection());
                    CaretManager.RemoveCaretsSelection();
                }
                else
                {
                    ActionManager.DoAction(new DeleteCharAction(EditDirection.Backward));
                }
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Enter)) // Enter
        {
            ActionManager.DoAction(new DeleteSelection());
            CaretManager.RemoveCaretsSelection();
            ActionManager.DoAction(new EnterAction());
        }
        else if (EnumFlag.Has(hk, HotKeys.Tab)) // Tab
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
                ActionManager.DoAction(new UnTabAction());
            else
                ActionManager.DoAction(new TabAction());
        }
        else if (EnumFlag.Has(hk, HotKeys.UpArrow)) // Move Up
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.Up);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Up);
                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.DownArrow)) // Move Down
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.Down);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Down);
                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.LeftArrow)) // Move Left
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.Left);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Left);
                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.RightArrow)) // Move Right
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.Right);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Right);
                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.PageUp)) // Move PageUp
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.PageUp);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.PageUp);
                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.PageDown)) // Move PageDown
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(CaretMove.PageDown);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.PageDown);
                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Home))
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
            }
            else
            {
                CaretManager.MoveCaretsPosition(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);

                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.End))
        {
            if (EnumFlag.Has(hk, HotKeys.Shift))
            {
                CaretManager.MoveCaretsAnchor(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
            }
            else
            {
                CaretManager.MoveCaretsPosition(
                    EnumFlag.Has(hk, HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);

                CaretManager.RemoveCaretsSelection();
            }
        }
        else if (EnumFlag.Has(hk, HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }
    }

    private void ProcessMouseInput()
    {
        MouseInput mouseInput = Backend.GetMouseInput();
        if (false == string.IsNullOrEmpty(_imeComposition))
            return;

        ConvertCoordinates(mouseInput.Position,
            out int _,
            out Coordinates coordinates,
            out bool isLineNumber,
            out bool isFold);

        if (isFold)
        {
            if (MouseAction.Click == mouseInput.LeftAction)
            {
                if (LineManager.GetLine(coordinates.LineIndex, out Line line))
                {
                    if (Folding.None != line.Folding)
                    {
                        line.Folding.Switch();
                        RowManager.OnRowChanged();
                        return;
                    }
                }
            }
        }

        if (mouseInput.Position.X > _viewPos.X && mouseInput.Position.X < _viewPos.X + _viewSize.X &&
            mouseInput.Position.Y > _viewPos.Y && mouseInput.Position.Y < _viewPos.Y + _viewSize.Y)
        {
            if (isFold)
            {
                Backend.SetMouseCursor(MouseCursor.Hand);
            }
            else if (isLineNumber)
            {
            }
            else
            {
                Backend.SetMouseCursor(MouseCursor.Beam);
            }
        }

        if (MouseAction.Click == mouseInput.LeftAction ||
            MouseAction.Click == mouseInput.MiddleAction)
        {
            _mouseDragStartText = coordinates;
        }

        if (MouseAction.Click == mouseInput.LeftAction)
        {
            if (_shiftPressed)
            {
                CaretManager.SingleCaret().AnchorPosition = coordinates;
            }
            else if (_altPressed)
            {
                CaretManager.AddCaret(coordinates);
            }
            else
            {
                CaretManager.SingleCaret(coordinates);
            }
        }
        else if (MouseAction.DoubleClick == mouseInput.LeftAction)
        {
            CaretManager.SingleCaret(coordinates);

            if (_shiftPressed)
            {
                // Select Line
                CaretManager.MoveCaretsPosition(CaretMove.EndOfLine);
                CaretManager.MoveCaretsAnchor(CaretMove.StartOfLine);
            }
            else
            {
                // Select word
                CaretManager.MoveCaretsPosition(CaretMove.EndOfWord);
                CaretManager.MoveCaretsAnchor(CaretMove.StartOfWord);
            }
        }
        else if (MouseAction.Dragging == mouseInput.LeftAction)
        {
            if (_altPressed)
            {
                CaretManager.SelectRectangle(_mouseDragStartText, coordinates);
            }
            else
            {
                CaretManager.SingleCaret(_mouseDragStartText).Position = coordinates;
            }
        }
        else if (MouseAction.Dragging == mouseInput.MiddleAction)
        {
            CaretManager.SelectRectangle(_mouseDragStartText, coordinates);
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

            foreach (Line line in LineManager.Lines)
            {
                line.CutoffsCache.SetDirty();
                line.LineSubsCache.SetDirty();
            }

            RowManager.RowsCache.SetDirty();
        }

        if (WrapMode.Word == WrapMode || WrapMode.BreakWord == WrapMode)
        {
            PageSize.X = _viewSize.X;
        }

        PageSize.Y = RowManager.Rows.Count * FontManager.GetLineHeight();
    }

    private int GetRowIndex(Vector2 viewCoordinates, int yOffset = 0)
    {
        float y = viewCoordinates.Y;

        int rowIndex = (int)(y / FontManager.GetLineHeight()) + yOffset;
        if (rowIndex < 0)
            rowIndex = 0;
        if (rowIndex >= RowManager.Rows.Count)
            rowIndex = RowManager.Rows.Count - 1;

        return rowIndex;
    }

    private void ConvertCoordinates(Vector2 viewCoordinates,
        out int rowIndex,
        out Coordinates coordinates,
        out bool isLineNumber,
        out bool isFold)
    {
        rowIndex = GetRowIndex(viewCoordinates);
        coordinates = new Coordinates(0, 0);
        isLineNumber = false;
        isFold = false;

        float x = viewCoordinates.X - LineNumberWidth - FoldWidth;
        if (x < -FoldWidth)
        {
            isLineNumber = true;
            return;
        }

        if (RowManager.Rows.Count > rowIndex)
        {
            Row row = RowManager.Rows[rowIndex];
            if (LineManager.GetLine(row.LineSub.Coordinates.LineIndex, out Line line))
            {
                coordinates.LineIndex = line.Index;

                if (x < -FontManager.GetFontWhiteSpaceWidth())
                {
                    if (Folding.None != line.Folding)
                    {
                        isFold = true;
                        return;
                    }
                }
                coordinates.CharIndex =
                    row.LineSub.Coordinates.CharIndex +
                    row.LineSub.GetCharIndex(x - row.IndentWidth);
                
                coordinates.LineSubIndex = row.LineSub.Coordinates.LineSubIndex;
            }
        }
    }
}