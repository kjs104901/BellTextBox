using Bell.Commands;
using Bell.Coordinates;
using Bell.Inputs;

namespace Bell;

public partial class TextBox
{
    public void Input(KeyboardInput keyboardInput, MouseInput mouseInput, ViewInput viewInput)
    {
        ProcessKeyboardHotKeys(keyboardInput.HotKeys);
        ProcessKeyboardChars(keyboardInput.Chars);
        ProcessMouseInput(keyboardInput.HotKeys, mouseInput);
        ProcessViewInput(viewInput);
    }
    

    private void ProcessKeyboardHotKeys(HotKeys hk)
    {
        if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Z)) // Undo
            _commandSetHistory.Undo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Y)) // Redo
            _commandSetHistory.Redo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.C)) // copy
            DoAction(new CopyCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.V)) // Paste
            DoAction(new PasteCommand());
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            CommandSet commandSet = new();
            commandSet.Add(new CopyCommand());
            commandSet.Add(new DeleteSelectionCommand());
            commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
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
            if (_cursor.HasSelection)
            {
                commandSet.Add(new DeleteSelectionCommand());
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            }
            else
            {
                if (hk.HasFlag(HotKeys.Shift))
                {
                    commandSet.Add(new DeleteLineCommand());
                }
                else
                {
                    commandSet.Add(new DeleteForwardCommand());
                }
            }
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Backspace)) // Backspace
        {
            CommandSet commandSet = new();
            if (_cursor.HasSelection)
            {
                commandSet.Add(new DeleteSelectionCommand());
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            }
            else
            {
                //TODO 시작이었다면 위로 머지
                commandSet.Add(new DeleteBackwardCommand());
            }
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Enter)) // Enter
        {
            CommandSet commandSet = new();
            if (_cursor.HasSelection)
            {
                commandSet.Add(new DeleteSelectionCommand());
                commandSet.Add(new MoveCursorSelectionCommand(CursorMove.Origin));
            }
            //TODO SplitLine
            //actionSet.Add(new InputChar('\n'));
            DoActionSet(commandSet);
        }
        else if (hk.HasFlag(HotKeys.Tab)) // Tab
        {
            CommandSet commandSet = new();
            if (_cursor.HasSelection)
            {
                commandSet.Add(new IndentSelection());
            }
            else
            {
                commandSet.Add(new InputChar('\t'));
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
    
    private void ProcessKeyboardChars(List<char> keyboardInputChars)
    {
        CommandSet commandSet = new();
        foreach (char keyboardInputChar in keyboardInputChars)
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

            commandSet.Add(new InputChar(keyboardInputChar));
        }
        DoActionSet(commandSet);
    }
    
    private void ProcessMouseInput(HotKeys hk, MouseInput mouseInput)
    {
        ViewCoordinates viewCoordinates = new(mouseInput.X, mouseInput.Y);
        if (false == viewCoordinates.ToPageCoordinates(
                Page,
                out PageCoordinates pageCoordinates,
                out bool isLine, out bool isMarker))
        {
            return;
        }

        if (isMarker)
        {
            //TODO find line and fold unfold

            return;
        }

        if (false == pageCoordinates.ToTextCoordinates(
                Page.Text,
                out TextCoordinates textCoordinates))
        {
            return;
        }

        if (MouseKey.Click == mouseInput.MouseKey)
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
        }
        else if (MouseKey.DoubleClick == mouseInput.MouseKey)
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
        else if (MouseKey.Dragging == mouseInput.MouseKey)
        {
            DoAction(new MoveCursorSelectionCommand(textCoordinates));
        }
    }

    private void ProcessViewInput(ViewInput viewInput)
    {
        ViewCoordinates start = new (viewInput.X, viewInput.Y);
        ViewCoordinates end = new (viewInput.X + viewInput.W, viewInput.Y + viewInput.H);
        Page.UpdateView(start, end);
    }
}