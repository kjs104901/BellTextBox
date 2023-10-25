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

    private int _rowStart;
    private int _rowEnd;

    public Vector2 PageSize;

    private LineCoordinates _mouseDragStartText;

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
            if (CaretManager.HasCaretsSelection())
            {
                ActionManager.DoAction(new DeleteSelection());
                CaretManager.RemoveCaretsSelection();
            }
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
    }

    private void ProcessMouseInput()
    {
        MouseInput mouseInput = _backend.GetMouseInput();

        ConvertCoordinates(mouseInput.Position,
            out int _,
            out LineCoordinates lineCoordinates,
            out bool isLineNumber,
            out bool isFold);
        
        if (isFold)
        {
            if (MouseAction.Click == mouseInput.LeftAction)
            {
                if (Folding.None != lineCoordinates.Line.Folding )
                {
                    lineCoordinates.Line.Folding.Switch();

                    LineManager.RowsCache.SetDirty();
                    CaretManager.SetCaretDirty();

                    return;
                }
            }
        }

        if (mouseInput.Position.X > _viewPos.X && mouseInput.Position.X < _viewPos.X + _viewSize.X &&
            mouseInput.Position.Y > _viewPos.Y && mouseInput.Position.Y < _viewPos.Y + _viewSize.Y)
        {
            if (isFold)
            {
                _backend.SetMouseCursor(MouseCursor.Hand);
            }
            else if (isLineNumber)
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
            _mouseDragStartText = lineCoordinates;
        }

        if (MouseAction.Click == mouseInput.LeftAction)
        {
            if (_shiftPressed)
            {
                CaretManager.SingleCaret().AnchorPosition = lineCoordinates;
            }
            else if (_altPressed)
            {
                CaretManager.AddCaret(lineCoordinates);
            }
            else
            {
                CaretManager.SingleCaret(lineCoordinates);
            }
        }
        else if (MouseAction.DoubleClick == mouseInput.LeftAction)
        {
            CaretManager.SingleCaret(lineCoordinates);

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
                CaretManager.SelectRectangle(_mouseDragStartText, lineCoordinates);
            }
            else
            {
                CaretManager.SingleCaret(_mouseDragStartText).Position = lineCoordinates;
            }
        }
        else if (MouseAction.Dragging == mouseInput.MiddleAction)
        {
            CaretManager.SelectRectangle(_mouseDragStartText, lineCoordinates);
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

            _rowStart = GetRowIndex(_viewPos, -3);
            _rowEnd = GetRowIndex(_viewPos + _viewSize, 3);
            
            foreach (Line line in LineManager.Lines)
            {
                line.CutoffsCache.SetDirty();
                line.SubLinesCache.SetDirty();
            }

            LineManager.RowsCache.SetDirty();
        }

        if (WrapMode.Word == WrapMode || WrapMode.BreakWord == WrapMode)
        {
            PageSize.X = _viewSize.X;
        }

        PageSize.Y = LineManager.Rows.Count * FontManager.GetLineHeight();
    }

    private int GetRowIndex(Vector2 viewCoordinates, int yOffset = 0)
    {
        float y = viewCoordinates.Y;
        
        int rowIndex = (int)(y / FontManager.GetLineHeight()) + yOffset;
        if (rowIndex < 0)
            rowIndex = 0;
        if (rowIndex >= LineManager.Rows.Count)
            rowIndex = LineManager.Rows.Count - 1;
        
        return rowIndex;
    }

    private void ConvertCoordinates(Vector2 viewCoordinates,
        out int rowIndex,
        out LineCoordinates lineCoordinates,
        out bool isLineNumber,
        out bool isFold)
    {
        rowIndex = GetRowIndex(viewCoordinates);
        lineCoordinates = new LineCoordinates { Line = Line.Empty, CharIndex = 0 };
        isLineNumber = false;
        isFold = false;
        
        float x = viewCoordinates.X - LineNumberWidth - FoldWidth;
        if (x < -FoldWidth)
        {
            isLineNumber = true;
            return;
        }
        
        if (LineManager.Rows.Count > rowIndex)
        {
            SubLine subLine = LineManager.Rows[rowIndex];
            Line line = subLine.LineCoordinates.Line;
            
            lineCoordinates.Line = line;

            if (x < - FontManager.GetFontWhiteSpaceWidth())
            {
                if (Folding.None != line.Folding)
                {
                    isFold = true;
                    return;
                }
            }

            lineCoordinates.CharIndex = 0;
            foreach (SubLine lineSubLine in line.SubLines)
            {
                if (subLine.WrapIndex <= lineSubLine.WrapIndex)
                    break;
                lineCoordinates.CharIndex += lineSubLine.Chars.Count;
            }
            lineCoordinates.CharIndex += subLine.GetCharIndex(x - subLine.IndentWidth);
        }
    }
    
    
    
    
    
    /*
    private Vector2 PageToView(TextCoordinates textCoordinates)
    {
        var view = new Vector2();

        if (LineManager.Rows.Count > textCoordinates.Row)
        {
            SubLine subLine = LineManager.Rows[textCoordinates.Row];

            int column = textCoordinates.Column;
            float pageX = 0.0f + subLine.TabWidth;
            foreach (var textBlockRender in subLine.TextBlockRenders)
            {
                if (column < textBlockRender.Text.Length)
                {
                    foreach (char c in textBlockRender.Text)
                    {
                        if (column < 1)
                            break;

                        column -= 1;
                        pageX += GetFontWidth(c);
                    }

                    break;
                }

                column -= textBlockRender.Text.Length;
                pageX += textBlockRender.Width;
            }

            view.X = Math.Max(view.X, pageX);
        }

        view.X += LineNumberWidth + FoldWidth;
        view.Y = textCoordinates.Row * GetFontHeight();

        return view;
    }
    */
}