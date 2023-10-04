namespace Bell.Inputs;

public enum MouseKey
{
    None,
    
    Click,
    DoubleClick,
    Dragging
}

public struct MouseInput
{
    public MouseKey MouseKey;

    public float X;
    public float Y;
}