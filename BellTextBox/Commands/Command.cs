using Bell.Data;

namespace Bell.Commands;

internal abstract class Command
{
    public abstract void Do(TextBox textBox);
}

internal abstract class EditCommand : Command
{
    public abstract void Undo(TextBox textBox);
}