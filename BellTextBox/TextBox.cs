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

    public void Render()
    {
        FontSizeManager.UpdateReferenceSize();

        var pageRender = new PageRender //TODO get from page
        {
            Size = new RectSize
            {
                Width = 500,
                Height = 800
            }
        };
        
        TextBoxBackend.Render(Input, pageRender, Page.Text.GetLineRenders());
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