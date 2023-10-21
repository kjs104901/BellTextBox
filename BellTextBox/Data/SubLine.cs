using Bell.Utils;

namespace Bell.Data;

public class SubLine
{
    private TextBox _textBox;

    public int LineIndex;
    public int SubIndex;

    public int Row;
    public Folding? Folding;

    public float TabWidth;

    public readonly List<TextBlockRender> TextBlockRenders = new();
    public readonly List<WhiteSpaceRender> WhiteSpaceRenders = new();

    public readonly List<float> CharWidths = new();

    public LineSelection LineSelection => LineSelectionCache.Get();
    public Cache<LineSelection> LineSelectionCache;

    public SubLine(TextBox textBox, int lineIndex, int subIndex, float tabWidth)
    {
        _textBox = textBox;

        LineIndex = lineIndex;
        SubIndex = subIndex;

        TabWidth = tabWidth;

        LineSelectionCache = new Cache<LineSelection>(new(), UpdateLineSelection);
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

    private LineSelection UpdateLineSelection(LineSelection lineSelection)
    {
        lineSelection.Selected = false;
        lineSelection.SelectionStart = 0.0f;
        lineSelection.SelectionEnd = 0.0f;

        lineSelection.CaretSelection = false;
        lineSelection.CaretSelectionPosition = 0.0f;
        lineSelection.CaretPosition = false;
        lineSelection.CaretPositionPosition = 0.0f;
        
        foreach (Caret caret in TextBox.Get().Carets)
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

                if (start.LineIndex == LineIndex)
                {
                    if (end.LineIndex > LineIndex)
                    {
                        lineSelection.SelectionStart = GetRenderPosition(start.Column);
                        lineSelection.SelectionEnd = CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                            lineSelection.SelectionEnd = 5.0f; //TODO get width of ' '

                        lineSelection.Selected = true;
                    }
                    else if (end.LineIndex == LineIndex)
                    {
                        lineSelection.SelectionStart = GetRenderPosition(start.Column);
                        lineSelection.SelectionEnd = GetRenderPosition(end.Column);
                        lineSelection.Selected = true;
                    }
                }
                else if (start.LineIndex < LineIndex)
                {
                    if (end.LineIndex > LineIndex)
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                            lineSelection.SelectionEnd = 5.0f; //TODO get width of ' '

                        lineSelection.Selected = true;
                    }
                    else if (end.LineIndex == LineIndex)
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = GetRenderPosition(end.Column);
                        lineSelection.Selected = true;
                    }
                }
            }

            if (caret.Selection.LineIndex == LineIndex)
            {
                lineSelection.CaretSelection = true;
                lineSelection.CaretSelectionPosition = GetRenderPosition(caret.Selection.Column);
            }

            if (caret.Position.LineIndex == LineIndex)
            {
                lineSelection.CaretPosition = true;
                lineSelection.CaretPositionPosition = GetRenderPosition(caret.Position.Column);
            }
        }

        return lineSelection;
    }
}

public struct LineSelection
{
    public bool Selected;
    public float SelectionStart;
    public float SelectionEnd;

    public bool CaretSelection;
    public float CaretSelectionPosition;
    public bool CaretPosition;
    public float CaretPositionPosition;
}

public struct TextBlockRender
{
    public string Text;
    public ColorStyle ColorStyle;

    public float PosX;
    public float Width;
}

public struct WhiteSpaceRender
{
    public char C;
    public float PosX;
}