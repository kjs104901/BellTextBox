namespace Bell.Inputs;

public enum MouseAction
{
    None,
    
    Click,
    DoubleClick,
    Dragging,
    
    MiddleDragging
}

public enum MouseCursor
{
    Arrow,
    Beam,
    Hand
}

public struct MouseInput
{
    public MouseAction LeftAction;
    public MouseAction MiddleAction;

    public float X;
    public float Y;
}