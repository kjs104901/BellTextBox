namespace Bell.Data;

public class LineRender
{
    public int LineIndex;
    public int WrapIndex;
    
    public int Row;
    
    public float TabWidth;

    public readonly List<TextBlockRender> TextBlockRenders = new();
    public readonly List<float> CharWidths = new();

    public bool CaretSet { get; private set; }
    
    public bool Selected { get; private set; }
    public float SelectionStart { get; private set; }
    public float SelectionEnd { get; private set; }
    
    public bool CaretSelection { get; private set; }
    public float CaretSelectionPosition { get; private set; }
    public bool CaretPosition { get; private set; }
    public float CaretPositionPosition { get; private set; }
    
    public LineRender(int lineIndex, int wrapIndex, float tabWidth)
    {
        LineIndex = lineIndex;
        WrapIndex = wrapIndex;
        
        TabWidth = tabWidth;
    }

    private float GetRenderPosition(int charIndex)
    {
        var position = 0.0f;
        for (var i = 0; i < charIndex; i++)
        {
            position += CharWidths[i];
        }
        return position;
    }
    
    public void SetCarets(List<Caret> carets)
    {
        CaretSet = true;
        
        Selected = false;
        SelectionStart = 0.0f;
        SelectionEnd = 0.0f;

        CaretSelection = false;
        CaretSelectionPosition = 0.0f;
        CaretPosition = false;
        CaretPositionPosition = 0.0f;
        
        foreach (Caret caret in carets)
        {
            if (caret.HasSelection)
            {
                TextCoordinates start;
                TextCoordinates end;
                if (caret.Selection < caret.Position)
                {
                    start = caret.Selection;
                    end = caret.Position;
                }
                else
                {
                    start = caret.Position;
                    end = caret.Selection;
                }
                
                if (start.Row == Row)
                {
                    if (end.Row > Row)
                    {
                        SelectionStart = GetRenderPosition(start.Column);
                        SelectionEnd = CharWidths.Sum();
                        if (SelectionEnd < 1.0f)
                            SelectionEnd = 5.0f; //TODO get width of ' '
                        
                        Selected = true;
                    }
                    else if (end.Row == Row)
                    {
                        SelectionStart = GetRenderPosition(start.Column);
                        SelectionEnd = GetRenderPosition(end.Column);
                        Selected = true;
                    }
                }
                else if (start.Row < Row)
                {
                    if (end.Row > Row)
                    {
                        SelectionStart = 0.0f;
                        SelectionEnd = CharWidths.Sum();
                        if (SelectionEnd < 1.0f)
                            SelectionEnd = 5.0f; //TODO get width of ' '
                        
                        Selected = true;
                    }
                    else if (end.Row == Row)
                    {
                        SelectionStart = 0.0f;
                        SelectionEnd = GetRenderPosition(end.Column);
                        Selected = true;
                    }
                }
            }
            
            if (caret.Selection.Row == Row)
            {
                CaretSelection = true;
                CaretSelectionPosition = GetRenderPosition(caret.Selection.Column);
            }
            
            if (caret.Position.Row == Row)
            {
                CaretPosition = true;
                CaretPositionPosition = GetRenderPosition(caret.Position.Column);
            }
        }
    }
}

public struct TextBlockRender
{
    public string Text;
    public ColorStyle ColorStyle;

    public float PosX;
    public float Width;
}