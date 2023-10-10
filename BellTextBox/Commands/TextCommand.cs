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

// TODO forward backward enum?

internal class InputForwardChar : EditCommand
{
    private char _c;
    public InputForwardChar(char c)
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

internal class UnindentSelection : EditCommand
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

internal class SplitLineForward : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class SplitLineBackward : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class MergeLineForward : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class MergeLineBackward : EditCommand
{
    public override void Do(TextBox textBox)
    {
    }

    public override void Undo(TextBox textBox)
    {
    }
}

internal class InputBackwardChar : EditCommand
{
    private char _c;
    public InputBackwardChar(char c)
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

internal class CopyCommand : Command
{
    public override void Do(TextBox textBox)
    {
    }
}

internal class PasteCommand : Command
{
    public override void Do(TextBox textBox)
    {
        //ClipboardText
    }
}