namespace Bell.Commands;

internal struct CommandSet
{
    private readonly List<Command> _actions = new();

    public CommandSet()
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
            if (action is EditCommand editAction)
            {
                editAction.Undo(textBox);
            }
        }
    }

    public bool HasEditAction => _actions.Any(a => a is EditCommand);

    public string GetDebugString()
    {
        return string.Join(",", _actions.Select(a => a.GetType().ToString()));
    }
}