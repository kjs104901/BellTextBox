using Bell.Commands;
using Bell.Coordinates;
using Bell.Inputs;

namespace Bell;

public partial class TextBox
{
    protected KeyboardInput KeyboardInput;
    protected MouseInput MouseInput;
    protected ViewInput ViewInput;

    public string ClipboardText = "";

    private TextCoordinates _debugTextCoordinates;
    
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
            CommandSetHistory.Undo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Y)) // Redo
            CommandSetHistory.Redo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.C)) // copy
            DoAction(new CopyCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.V)) // Paste
            DoAction(new PasteCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            CommandSet commandSet = new();
            commandSet.Add(new CopyCommand());
            //TODO Delete select
            //commandSet.Add(new DeleteSelectionCommand());
            //commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorSelectionCommand(CursorMove.StartOfFile));
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.EndOfFile));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Delete)) // Delete
        {
            CommandSet commandSet = new();
            if (Cursor.HasSelection)
            {
                //TODO Delete select
                //commandSet.Add(new DeleteSelectionCommand());
                //commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            }
            else
            {
                if (hk.HasFlag(HotKeys.Shift))
                {
                    //commandSet.Add(new DeleteLineCommand());
                }
                else
                {
                    commandSet.Add(new DeleteCommand(EditDirection.Forward, 1));
                }
            }

            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Backspace)) // Backspace
        {
            CommandSet commandSet = new();
            if (Cursor.HasSelection)
            {
                //TODO Delete select
                //commandSet.Add(new DeleteSelectionCommand());
                //commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            }
            else
            {
                //TODO 시작이었다면 위로 머지
                commandSet.Add(new DeleteCommand(EditDirection.Backward, 1));
            }

            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Enter)) // Enter
        {
            CommandSet commandSet = new();
            if (Cursor.HasSelection)
            {
                //TODO Delete select
                //commandSet.Add(new DeleteSelectionCommand());
                //commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            }

            //TODO SplitLine
            //actionSet.Add(new InputChar('\n'));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Tab)) // Tab
        {
            CommandSet commandSet = new();
            if (Cursor.HasSelection)
            {
                if (hk.HasFlag(HotKeys.Shift))
                    commandSet.Add(new UnindentSelection());
                else
                    commandSet.Add(new IndentSelection());
            }
            else
            {
                commandSet.Add(new InputChar(EditDirection.Forward, new[] { '\t' }));
            }

            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.UpArrow)) // Move Up
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.Up));
            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.DownArrow)) // Move Down
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.Down));
            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.LeftArrow)) // Move Left
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.Left));
            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.RightArrow)) // Move Right
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.Right));
            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.PageUp)) // Move PageUp
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.PageUp));
            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.PageDown)) // Move PageDown
        {
            CommandSet commandSet = new();
            commandSet.Add(new MoveCursorOriginCommand(CursorMove.PageDown));
            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Home))
        {
            CommandSet commandSet = new();

            commandSet.Add(hk.HasFlag(HotKeys.Ctrl)
                ? new MoveCursorOriginCommand(CursorMove.EndOfFile)
                : new MoveCursorOriginCommand(CursorMove.EndOfLine));

            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.End))
        {
            CommandSet commandSet = new();

            commandSet.Add(hk.HasFlag(HotKeys.Ctrl)
                ? new MoveCursorOriginCommand(CursorMove.StartOfFile)
                : new MoveCursorOriginCommand(CursorMove.StartOfLine));

            if (false == hk.HasFlag(HotKeys.Shift))
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }
    }

    private void ProcessKeyboardChars()
    {
        CommandSet commandSet = new();
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

            commandSet.Add(new InputChar(EditDirection.Forward, new[] { keyboardInputChar }));
        }

        DoActionSet(commandSet);
    }

    private void DeleteSelection()
    {
        // delete forward Cursor~
        // merge line forward
        // delete forward Cursor~
        // ...
    }

    private void ProcessMouseInput()
    {
        var hk = KeyboardInput.HotKeys;
        
        var viewCoordinates = new ViewCoordinates() { X = MouseInput.X, Y = MouseInput.Y };
        var pageCoordinates = CoordinatesManager.Convert(viewCoordinates);
        var textCoordinates = CoordinatesManager.Convert(pageCoordinates);

        if (textCoordinates.IsMarker)
        {
            if (MouseKey.Click == MouseInput.MouseKey)
            {
                //TODO fold unfold
            }
            return;
        }

        if (MouseKey.Click == MouseInput.MouseKey)
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                DoAction(new MoveCursorSelectionCommand(textCoordinates));
            }
            else
            {
                CommandSet commandSet = new();
                commandSet.Add(new MoveCursorSelectionCommand(textCoordinates));
                commandSet.Add(new MoveCursorOriginCommand(textCoordinates));
                DoActionSet(commandSet);
            }

            _debugTextCoordinates = textCoordinates;
        }
        else if (MouseKey.DoubleClick == MouseInput.MouseKey)
        {
            if (hk.HasFlag(HotKeys.Shift)) // Select Line
            {
                CommandSet commandSet = new();
                commandSet.Add(new MoveCursorOriginCommand(textCoordinates));

                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.StartOfLine));
                commandSet.Add(new MoveCursorOriginCommand(CursorMove.EndOfLine));
                DoActionSet(commandSet);
            }
            else // Select word
            {
                CommandSet commandSet = new();
                commandSet.Add(new MoveCursorOriginCommand(textCoordinates));

                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.StartOfWord));
                commandSet.Add(new MoveCursorOriginCommand(CursorMove.EndOfWord));
                DoActionSet(commandSet);
            }
        }
        else if (MouseKey.Dragging == MouseInput.MouseKey)
        {
            DoAction(new MoveCursorSelectionCommand(textCoordinates));
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