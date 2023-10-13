namespace Bell.Actions;

internal struct Action
{
    private readonly List<Command> _actions = new();

    public Action()
    {
    }

    public void Add(Command command)
    {
        _actions.Add(command);
    }

    public void Do(TextBox textBox)
    {
        foreach (Command action in _actions)
        {
            action.Do(textBox);
        }
    }

    public void Undo(TextBox textBox)
    {
        foreach (Command action in _actions)
        {
            action.Undo(textBox);
        }
    }
    
    public string GetDebugString()
    {
        return string.Join(",", _actions.Select(a => a.GetType().ToString()));
    }
}