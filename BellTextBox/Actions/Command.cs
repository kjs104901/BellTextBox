namespace Bell.Actions;

internal abstract class Command
{
    public abstract void Do(TextBox textBox);
    public abstract void Undo(TextBox textBox);
}