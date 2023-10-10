using System.Text;
using Bell.Commands;
using Bell.Data;
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
    
    public ITextBoxBackend TextBoxBackend { get; set; }
    
    public TextBox(ITextBoxBackend textBoxBackend)
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
        TextBoxBackend.Render(Input, Page.Render, Page.Text.GetLineRenders());
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