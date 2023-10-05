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
    private readonly CommandSetHistory _commandSetHistory = new();
    private readonly Cursor _cursor = new();
    
    public ITextBoxBackend TextBoxBackend { get; set; }
    
    public TextBox(ITextBoxBackend textBoxBackend)
    {
        TextBoxBackend = textBoxBackend;
        Page = new Page(this);
        FontSizeManager = new FontSizeManager(this);
    }
    
    // Method
    public List<uint> FindText(string text)
    {
        return new();
    }

    public bool Goto(uint lineNumber)
    {
        return true;
    }
    
    public bool Fold(uint lineNumber)
    {
        return true;
    }

    public bool Unfold(uint lineNumber)
    {
        return true;
    }

    public void Render()
    {
        FontSizeManager.UpdateReferenceSize();
        TextBoxBackend.Render(this, Page.Text.GetLineRenders());
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
        _commandSetHistory.AddHistory(commandSet);
    }
}