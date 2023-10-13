using Bell.Commands;
using Bell.Coordinates;
using Bell.Inputs;
using Action = Bell.Commands.Action;

namespace Bell;

public partial class TextBox
{
    protected KeyboardInput KeyboardInput;
    protected MouseInput MouseInput;
    protected ViewInput ViewInput;

    public string ClipboardText = "";

    private TextCoordinates _debugTextCoordinates; // TODO Delete
    
    protected void InputStart()
    {
        KeyboardInput.HotKeys = HotKeys.None;
        KeyboardInput.Chars.Clear();
        KeyboardInput.ImeComposition = string.Empty;

        MouseInput.X = 0.0f;
        MouseInput.Y = 0.0f;
        MouseInput.MouseKey = MouseKey.None;
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
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.C)) // copy
            DoAction(new CopyCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.V)) // Paste
            DoAction(new PasteCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            Action action = new();
            action.Add(new CopyCommand());
            //TODO Delete select
            //action.Add(new DeleteSelectionCommand());
            //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            Action action = new();
            action.Add(new MoveCaretSelectionCommand(CaretMove.StartOfFile));
            action.Add(new MoveCaretPositionCommand(CaretMove.EndOfFile));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Delete)) // Delete
        {
            Action action = new();
            if (CaretManager.HasSelection())
            {
                //TODO Delete select
                //action.Add(new DeleteSelectionCommand());
                //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            }
            else
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
            if (CaretManager.HasSelection())
            {
                //TODO Delete select
                //action.Add(new DeleteSelectionCommand());
                //action.Add(new MoveCaretSelectionCommand(Caret.Position));
            }
            else
            {
                //TODO 시작이었다면 위로 머지
                action.Add(new DeleteCommand(EditDirection.Backward, 1));
            }

            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Enter)) // Enter
        {
            Action action = new();
            if (CaretManager.HasSelection())
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
            if (CaretManager.HasSelection())
            {
                if (hk.HasFlag(HotKeys.Shift))
                    action.Add(new UnindentSelection());
                else
                    action.Add(new IndentSelection());
            }
            else
            {
                action.Add(new InputChar(EditDirection.Forward, new[] { '\t' }));
            }

            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.UpArrow)) // Move Up
        {
            Action action = new();
            action.Add(new MoveCaretPositionCommand(CaretMove.Up));
            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.DownArrow)) // Move Down
        {
            Action action = new();
            action.Add(new MoveCaretPositionCommand(CaretMove.Down));
            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.LeftArrow)) // Move Left
        {
            Action action = new();
            action.Add(new MoveCaretPositionCommand(CaretMove.Left));
            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.RightArrow)) // Move Right
        {
            Action action = new();
            action.Add(new MoveCaretPositionCommand(CaretMove.Right));
            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.PageUp)) // Move PageUp
        {
            Action action = new();
            action.Add(new MoveCaretPositionCommand(CaretMove.PageUp));
            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.PageDown)) // Move PageDown
        {
            Action action = new();
            action.Add(new MoveCaretPositionCommand(CaretMove.PageDown));
            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.Home))
        {
            Action action = new();

            action.Add(hk.HasFlag(HotKeys.Ctrl)
                ? new MoveCaretPositionCommand(CaretMove.EndOfFile)
                : new MoveCaretPositionCommand(CaretMove.EndOfLine));

            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
            DoActionSet(action);
        }
        else if (hk.HasFlag(HotKeys.End))
        {
            Action action = new();

            action.Add(hk.HasFlag(HotKeys.Ctrl)
                ? new MoveCaretPositionCommand(CaretMove.StartOfFile)
                : new MoveCaretPositionCommand(CaretMove.StartOfLine));

            if (false == hk.HasFlag(HotKeys.Shift))
                action.Add(new MoveCaretSelectionCommand(CaretMove.Position));
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
        
        var viewCoordinates = new ViewCoordinates() { X = MouseInput.X, Y = MouseInput.Y };
        var pageCoordinates = CoordinatesManager.ViewToPage(viewCoordinates);
        var textCoordinates = CoordinatesManager.PageToText(pageCoordinates);

        if (textCoordinates.IsFold)
        {
            if (MouseKey.Click == MouseInput.MouseKey)
            {
                //TODO fold unfold
                return;
            }
        }

        var vx = MouseInput.X - ViewInput.X;
        var vy = MouseInput.Y - ViewInput.Y;

        if (vx > 0 && vx < ViewInput.W && vy > 0 && vy < ViewInput.H)
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
            
        if (MouseKey.Click == MouseInput.MouseKey)
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                DoAction(new MoveCaretSelectionCommand(textCoordinates));
            }
            else
            {
                Action action = new();
                action.Add(new MoveCaretSelectionCommand(textCoordinates));
                action.Add(new MoveCaretPositionCommand(textCoordinates));
                DoActionSet(action);
            }

            _debugTextCoordinates = textCoordinates;
            CaretManager.SetCaret(textCoordinates);
        }
        else if (MouseKey.DoubleClick == MouseInput.MouseKey)
        {
            if (hk.HasFlag(HotKeys.Shift)) // Select Line
            {
                Action action = new();
                action.Add(new MoveCaretPositionCommand(textCoordinates));

                action.Add(new MoveCaretSelectionCommand(CaretMove.StartOfLine));
                action.Add(new MoveCaretPositionCommand(CaretMove.EndOfLine));
                DoActionSet(action);
            }
            else // Select word
            {
                Action action = new();
                action.Add(new MoveCaretPositionCommand(textCoordinates));

                action.Add(new MoveCaretSelectionCommand(CaretMove.StartOfWord));
                action.Add(new MoveCaretPositionCommand(CaretMove.EndOfWord));
                DoActionSet(action);
            }
        }
        else if (MouseKey.Dragging == MouseInput.MouseKey)
        {
            DoAction(new MoveCaretSelectionCommand(textCoordinates));
        }
    }

    private void ProcessViewInput()
    {
        var start = new ViewCoordinates()
        {
            X = ViewInput.X,
            Y = ViewInput.Y
        };
        var end = new ViewCoordinates()
        {
            X = ViewInput.X + ViewInput.W,
            Y = ViewInput.Y + ViewInput.H
        };
        Page.UpdateView(start, end);
    }
}