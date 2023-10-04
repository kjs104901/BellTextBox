using System.Text;
using Bell.Commands;
using Bell.Data;

namespace Bell;

public partial class TextBox
{
    // Data
    public Page Page { get; set; }
    public readonly List<string> AutoCompleteList = new();
    public readonly StringBuilder StringBuilder = new();
    
    // Action
    private readonly CommandSetHistory _commandSetHistory = new();
    private readonly Cursor _cursor = new();
    
    private readonly ITextBoxBackend _textBoxBackend;
    
    public TextBox(ITextBoxBackend textBoxBackend)
    {
        _textBoxBackend = textBoxBackend;
        Page = new Page(this);
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
        _textBoxBackend.Render(this, Page.Text.LineRenders);
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