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
            ActionManager.Undo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.Y)) // Redo
            ActionManager.Redo();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.C)) // TODO Copy
            CaretManager.CopyClipboard();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.V)) // TODO Paste
            CaretManager.PasteClipboard();
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.X)) // Cut
        {
            CaretManager.CopyClipboard();
            ActionManager.Do(new DeleteSelection(this));
        }
        else if (hk.HasFlag(HotKeys.Ctrl | HotKeys.A)) // Select All
        {
            CaretManager.MoveCaretsSelection(CaretMove.StartOfFile);
            CaretManager.MoveCaretsPosition(CaretMove.EndOfFile);
        }
        else if (hk.HasFlag(HotKeys.Delete)) // Delete
        {
            ActionManager.Do(new DeleteSelection(this));
            ActionManager.Do(new DeleteCharAction(this, EditDirection.Forward));
        }
        else if (hk.HasFlag(HotKeys.Backspace)) // Backspace
        {
            ActionManager.Do(new DeleteSelection(this));
            ActionManager.Do(new DeleteCharAction(this, EditDirection.Backward));
        }
        else if (hk.HasFlag(HotKeys.Enter)) // Enter
        {
            ActionManager.Do(new DeleteSelection(this));
            ActionManager.Do(new EnterAction(this));
        }
        else if (hk.HasFlag(HotKeys.Tab)) // Tab
        {
            if (hk.HasFlag(HotKeys.Shift))
                ActionManager.Do(new UnTabAction(this));
            else
                ActionManager.Do(new TabAction(this));
        }
        else if (hk.HasFlag(HotKeys.UpArrow)) // Move Up
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(CaretMove.Up);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Up);
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.DownArrow)) // Move Down
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(CaretMove.Down);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Down);
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.LeftArrow)) // Move Left
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(CaretMove.Left);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Left);
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.RightArrow)) // Move Right
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(CaretMove.Right);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.Right);
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.PageUp)) // Move PageUp
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(CaretMove.PageUp);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.PageUp);
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.PageDown)) // Move PageDown
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(CaretMove.PageDown);
            }
            else
            {
                CaretManager.MoveCaretsPosition(CaretMove.PageDown);
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.Home))
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
            }
            else
            {
                CaretManager.MoveCaretsPosition(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.StartOfFile : CaretMove.StartOfLine);
                
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.End))
        {
            if (hk.HasFlag(HotKeys.Shift))
            {
                CaretManager.MoveCaretsSelection(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
            }
            else
            {
                CaretManager.MoveCaretsPosition(
                    hk.HasFlag(HotKeys.Ctrl) ? CaretMove.EndOfFile : CaretMove.EndOfLine);
                
                CaretManager.MoveCaretsSelection(CaretMove.Position);
            }
        }
        else if (hk.HasFlag(HotKeys.Insert))
        {
            Overwrite = !Overwrite;
        }
    }

    private void ProcessKeyboardChars()
    {
        var hk = KeyboardInput.HotKeys;
        bool selectionDeleted = false;
        
        foreach (char keyboardInputChar in KeyboardInput.Chars)
        {
            if (keyboardInputChar == 0)
                continue;

            if (keyboardInputChar == '\n')
            {
                ActionManager.Do(new EnterAction(this));
                continue;
            }

            if (keyboardInputChar == '\t')
            {
                if (hk.HasFlag(HotKeys.Shift))
                    ActionManager.Do(new UnTabAction(this));
                else
                    ActionManager.Do(new TabAction(this));
                continue;
            }

            if (keyboardInputChar < 32)
                continue;

            if (false == selectionDeleted)
            {
                ActionManager.Do(new DeleteSelection(this));
                selectionDeleted = true;
            }
            ActionManager.Do(new InputCharAction(this, EditDirection.Forward)); // keyboardInputChar
        }
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