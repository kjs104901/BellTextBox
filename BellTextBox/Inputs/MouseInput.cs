namespace Bell.Inputs;

public enum MouseKey
{
    None,
    
    Click,
    DoubleClick,
    Dragging
}

public enum MouseCursor
{
    Arrow,
    Beam,
    Hand
}

public struct MouseInput
{
    public MouseKey MouseKey;

    public float X;
    public float Y;
}