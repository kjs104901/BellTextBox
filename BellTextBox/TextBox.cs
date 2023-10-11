using System.Numerics;
using System.Text;
using Bell.Commands;
using Bell.Data;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public partial class TextBox
{
    // Data
    public Page Page { get; set; }
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();
    public FontSizeManager FontSizeManager { get; set; }

    // Action
    internal readonly CommandSetHistory CommandSetHistory = new();
    internal readonly Cursor Cursor = new();

    public TextBoxBackend TextBoxBackend { get; set; }

    public TextBox(TextBoxBackend textBoxBackend)
    {
        TextBoxBackend = textBoxBackend;
        Page = new Page(this);
        FontSizeManager = new FontSizeManager(this);
    }

    // Method
    public List<int> FindText(string text)
    {
        return new();
    }

    public bool Goto(int lineNumber)
    {
        return true;
    }

    public bool Fold(int lineNumber)
    {
        return true;
    }

    public bool Unfold(int lineNumber)
    {
        return true;
    }

    public void SetText(string text)
    {
        Page.SetText(text);
    }

    public void Render()
    {
        FontSizeManager.UpdateReferenceSize();

        TextBoxBackend.StartTextBox();
        TextBoxBackend.Input();
        
        ProcessKeyboardHotKeys(TextBoxBackend.KeyboardInput.HotKeys);
        ProcessKeyboardChars(TextBoxBackend.KeyboardInput.Chars);
        ProcessMouseInput(TextBoxBackend.KeyboardInput.HotKeys, TextBoxBackend.MouseInput);
        ProcessViewInput(TextBoxBackend.ViewInput);

        TextBoxBackend.PageRender = Page.Render;
        TextBoxBackend.StartPage();
        foreach (LineRender lineRender in Page.LineRenders)
        {
            float width = 0.0f;
            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                TextBoxBackend.RenderText(
                    new Vector2(lineRender.PosX + width, lineRender.PosY),
                    textBlockRender.Text,
                    textBlockRender.FontStyle);
                width += textBlockRender.Width;
            }
        }
        TextBoxBackend.EndPage();
        TextBoxBackend.EndTextBox();
    }

    private void DoAction(Command command)
    {
        CommandSet commandSet = new CommandSet();
        commandSet.Add(command);
        DoActionSet(commandSet);
    }

    private void DoActionSet(CommandSet commandSet)
    {
        commandSet.Do(this);
        CommandSetHistory.AddHistory(commandSet);
    }
}