using System.Numerics;
using System.Text;
using Bell.Commands;
using Bell.Coordinates;
using Bell.Data;
using Bell.Languages;
using Bell.Render;

namespace Bell;

public partial class TextBox : IDisposable
{
    // Data
    public Page? Page { get; private set; }
    public Text? Text { get; private set; }
    
    
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();
    public FontSizeManager? FontSizeManager { get; private set; }
    public CoordinatesManager? CoordinatesManager { get; private set; }

    // Action
    internal readonly CommandSetHistory CommandSetHistory = new();
    internal readonly Cursor Cursor = new();

    public TextBoxBackend TextBoxBackend { get; set; }

    public TextBox(TextBoxBackend textBoxBackend)
    {
        TextBoxBackend = textBoxBackend;

        Page = new Page(this);
        Text = new Text(this);
        
        FontSizeManager = new FontSizeManager(this);
        CoordinatesManager = new CoordinatesManager(this);
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
        if (null == Page || null == Text)
            return;
        
        Text.SetText(text);
        Page.RenderCache.SetDirty();
    }

    public void Render()
    {
        if (null == Page || null == Text || null == FontSizeManager)
            return;
        
        FontSizeManager.UpdateReferenceSize();

        TextBoxBackend.Begin();
        TextBoxBackend.Input();
        
        ProcessKeyboardHotKeys(TextBoxBackend.KeyboardInput.HotKeys);
        ProcessKeyboardChars(TextBoxBackend.KeyboardInput.Chars);
        ProcessMouseInput(TextBoxBackend.KeyboardInput.HotKeys, TextBoxBackend.MouseInput);
        ProcessViewInput(TextBoxBackend.ViewInput);

        TextBoxBackend.PageRender = Page.Render;
        foreach (LineRender lineRender in Page.LineRenders)
        {
            float width = 0.0f;
            foreach (TextBlockRender textBlockRender in lineRender.TextBlockRenders)
            {
                TextBoxBackend.RenderText(
                    new Vector2(LineNumberWidth + MarkerWidth + lineRender.PosX + width, lineRender.PosY),
                    textBlockRender.Text,
                    textBlockRender.FontStyle);
                width += textBlockRender.Width;
            }
        }
        TextBoxBackend.RenderText(new Vector2(150, 0), $"{_debugTextCoordinates.Row} {_debugTextCoordinates.Column}", FontStyle.DefaultFontStyle);
        
        TextBoxBackend.End();
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

    public void Dispose()
    {
        Page?.Dispose();
        Page = null;
        
        Text?.Dispose();
        Text = null;
        
        FontSizeManager?.Dispose();
        FontSizeManager = null;
        
        CoordinatesManager?.Dispose();
        CoordinatesManager = null;
    }
}