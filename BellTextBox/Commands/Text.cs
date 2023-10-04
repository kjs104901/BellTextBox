namespace Bell.Commands;

// InputChar
// InputString

// DeleteBackward
// DeleteForward

// IndentSelection
// DeleteSelection

// SplitLine
// MergeForwardLine
// MergeBackwardLine
// DeleteLine

/* undo 정보
    public string? Added;
    public Coordinates AddedStart;
    public Coordinates AddedEnd;

    public string? Removed;
    public Coordinates RemovedStart;
    public Coordinates RemovedEnd;

    public SelectionState Before;
    public SelectionState After;

 */

internal class InputChar : EditCommand
{
    private char _c;
    public InputChar(char c)
    {
        _c = c;
    }
    
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class IndentSelection : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class DeleteSelectionCommand : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class DeleteForwardCommand : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class DeleteBackwardCommand : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class DeleteLineCommand : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}


internal class CopyCommand : Command
{
    public override void Do(TextBox textBox)
    {
    }
}

internal class PasteCommand : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}