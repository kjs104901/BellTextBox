using System.Numerics;
using Bell.Actions;
using Bell.Carets;
using Bell.Coordinates;
using Bell.Inputs;
using Action = Bell.Actions.Action;

namespace Bell;

public partial class TextBox
{
    protected KeyboardInput KeyboardInput;
    protected MouseInput MouseInput;
    protected ViewInput ViewInput;

    public Vector2 ViewStart;
    public Vector2 ViewEnd;
    public float PageWidth;
    
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

        if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Z)) // Undo
            ActionHistory.Undo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Y)) // Redo
            ActionHistory.Redo();
        //else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.C)) // copy
        //    DoAction(new CopyCommand());
        //else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.V)) // Paste
        //    DoAction(new PasteCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            Action action = new();
            //action.Add(new CopyCommand());
            //TODO Delete select
            //action.Add(new DeleteSelectionCommand());
            //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            Action action = new();
            //action.Add(new MoveCaretSelectionCommand(CaretMove.StartOfFile));
            //action.Add(new MoveCaretPositionCommand(CaretMove.EndOfFile));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Delete)) // Delete
        {
            Action action = new();
            //if (CaretManager.HasSelection())
            {
                //TODO Delete select
                //action.Add(new DeleteSelectionCommand());
                //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            }
            //else
            {
                if (hk.HasFlag(HotKeys.Shift))
                {
                    //action.Add(new DeleteLineCommand());
                }
                else
                {
                    action.Add(new DeleteCommand(EditDirection.Forward, 1));
                }
            }

            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Backspace)) // Backspace
        {
            Action action = new();
            //if (CaretManager.HasSelection())
            {
                //TODO Delete select
                //action.Add(new DeleteSelectionCommand());
                //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            }
            //else
            {
                //TODO 시작이었다면 위로 머지
                action.Add(new DeleteCommand(EditDirection.Backward, 1));
            }

            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Enter)) // Enter
        {
            Action action = new();
            //if (CaretManager.HasSelection())
            {
                //TODO Delete select
                //action.Add(new DeleteSelectionCommand());
                //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            }

            //TODO SplitLine
            //actionSet.Add(new InputChar('\n'));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Tab)) // Tab
        {
            Action action = new();
            //if (CaretManager.HasSelection())
            {
                if (hk.HasFlag(HotKeys.Shift))
                    action.Add(new UnindentSelection());
                else
                    action.Add(new IndentSelection());
            }
            //else
            {
                if (hk.HasFlag(HotKeys.Shift))
                    //TODO Delete indent action.Add(new InputChar(EditDirection.Backward, new[] { '\t' }));
                else
                    action.Add(new InputChar(EditDirection.Forward, new[] { '\t' }));
            }

            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.UpArrow)) // Move Up
        {
            Action action = new();
            //action.Add(new MoveCaretPositionCommand(CaretMove.Up));
            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.DownArrow)) // Move Down
        {
            Action action = new();
            //action.Add(new MoveCaretPositionCommand(CaretMove.Down));
            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.LeftArrow)) // Move Left
        {
            Action action = new();
            //action.Add(new MoveCaretPositionCommand(CaretMove.Left));
            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.RightArrow)) // Move Right
        {
            Action action = new();
            //action.Add(new MoveCaretPositionCommand(CaretMove.Right));
            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.PageUp)) // Move PageUp
        {
            Action action = new();
            //action.Add(new MoveCaretPositionCommand(CaretMove.PageUp));
            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.PageDown)) // Move PageDown
        {
            Action action = new();
            //action.Add(new MoveCaretPositionCommand(CaretMove.PageDown));
            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Home))
        {
            Action action = new();

            //action.Add(hk.HasFlag(HotKeys.Ctrl)
            //    ? new MoveCaretPositionCommand(CaretMove.EndOfFile)
            //    : new MoveCaretPositionCommand(CaretMove.EndOfLine));

            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.End))
        {
            Action action = new();

            //action.Add(hk.HasFlag(HotKeys.Ctrl)
            //    ? new MoveCaretPositionCommand(CaretMove.StartOfFile)
            //    : new MoveCaretPositionCommand(CaretMove.StartOfLine));

            if (false == hk.HasFlag(HotKeys.Shift))
                //    action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
                DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }
    }

    private void ProcessKeyboardChars()
    {
        Action action = new();
        foreach (char keyboardInputChar in KeyboardInput.Chars)
        {
            if (keyboardInputChar == 0)
                continue;

            if (keyboardInputChar == '\n')
            {
                //Todo enter
                continue;
            }

            if (keyboardInputChar == '\t')
            {
                //Todo tab
                continue;
            }

            if (keyboardInputChar < 32)
                continue;

            action.Add(new InputChar(EditDirection.Forward, new[] { keyboardInputChar }));
        }

        DoActionSet(action);
    }

    private void DeleteSelection()
    {
        // delete forward Caret~
        // merge line forward
        // delete forward Caret~
        // ...
    }

    private void ProcessMouseInput()
    {
        var hk = KeyboardInput.HotKeys;

        Vector2 pageCoordinates = CoordinatesManager.ViewToPage(MouseInput.Position);
        TextCoordinates textCoordinates = CoordinatesManager.PageToText(pageCoordinates);

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
            else if (textCoordinates.IsLineNumber) { }
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
                CaretManager.SingleCaret().Selection = textCoordinates;
            }
            else if (hk.HasFlag(HotKeys.Alt))
            {
                CaretManager.AddCaret(textCoordinates);
            }
            else
            {
                CaretManager.SingleCaret(textCoordinates);
            }
        }
        else if (MouseAction.DoubleClick == MouseInput.LeftAction)
        {
            CaretManager.SingleCaret(textCoordinates);

            if (hk.HasFlag(HotKeys.Shift))
            {
                // Select Line
                CaretManager.MoveCaretsPosition(CaretMove.EndOfLine);
                CaretManager.MoveCaretsSelection(CaretMove.StartOfLine);
            }
            else
            {
                // Select word
                CaretManager.MoveCaretsPosition(CaretMove.EndOfWord);
                CaretManager.MoveCaretsSelection(CaretMove.StartOfWord);
            }
        }
        else if (MouseAction.Dragging == MouseInput.LeftAction)
        {
            if (hk.HasFlag(HotKeys.Alt))
            {
                CaretManager.SelectRectangle(_mouseDragStartPage, pageCoordinates);
            }
            else
            {
                CaretManager.SingleCaret(_mouseDragStartText).Position = textCoordinates;
            }
        }
        else if (MouseAction.Dragging == MouseInput.MiddleAction)
        {
            CaretManager.SelectRectangle(_mouseDragStartPage, pageCoordinates);
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

        if (Math.Abs(PageWidth - (ViewInput.W - LineNumberWidthMax - FoldWidth)) > 1.0f)
        {
            PageWidth = ViewInput.W;
            Page.LineRendersCache.SetDirty();
        }
    }
}