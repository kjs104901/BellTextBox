﻿using Bell.Utils;

namespace Bell.Data;

public class SubLine
{
    public int Row;
    
    public int LineIndex;
    public int WrapIndex;

    public int StartCharIndex;

    public Folding? Folding;

    public float TabWidth;

    public readonly List<TextBlockRender> TextBlockRenders = new();
    public readonly List<WhiteSpaceRender> WhiteSpaceRenders = new();

    public readonly List<char> Chars = new();
    public readonly List<float> CharWidths = new();

    public LineSelection LineSelection => LineSelectionCache.Get();
    public readonly Cache<LineSelection> LineSelectionCache;

    public SubLine(int lineIndex, int wrapIndex, int startCharIndex, float tabWidth)
    {
        LineIndex = lineIndex;
        WrapIndex = wrapIndex;
        StartCharIndex = startCharIndex;

        TabWidth = tabWidth;

        LineSelectionCache = new Cache<LineSelection>(new(), UpdateLineSelection);
    }

    private bool IsSameSubLine(TextCoordinates coordinates, out float position)
    {
        position = 0.0f;
        
        if (coordinates.LineIndex != LineIndex)
            return false;
        
        var index = coordinates.CharIndex - StartCharIndex;
        if (index < 0 || index >= CharWidths.Count)
            return false;
        
        position = 0.0f;
        for (var i = 0; i < index; i++)
        {
            position += CharWidths[i];
        }
        return true;
    }

    private int CompareSubLine(TextCoordinates coordinates)
    {
        if (coordinates.LineIndex < LineIndex)
            return 1;

        if (coordinates.LineIndex > LineIndex)
            return -1;
        
        var index = coordinates.CharIndex - StartCharIndex;
        if (index < 0)
            return 1;

        if (index >= CharWidths.Count)
            return -1;

        return 0;
    }
    
    private LineSelection UpdateLineSelection(LineSelection lineSelection)
    {
        lineSelection.Selected = false;
        lineSelection.SelectionStart = 0.0f;
        lineSelection.SelectionEnd = 0.0f;

        lineSelection.HasCaretAnchor = false;
        lineSelection.CaretAnchorPosition = 0.0f;
        lineSelection.HasCaret = false;
        lineSelection.CaretPosition = 0.0f;
        
        foreach (Caret caret in ThreadLocal.TextBox.Carets)
        {
            if (caret.HasSelection)
            {
                caret.GetSortedSelection(out var start, out var end);

                float startPosition;
                float endPosition;

                if (IsSameSubLine(start, out startPosition))
                {
                    if (CompareSubLine(end) < 0)
                    {
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                            lineSelection.SelectionEnd = 5.0f; //TODO get width of ' '

                        lineSelection.Selected = true;
                    }
                    else if (IsSameSubLine(end, out endPosition))
                    {
                        lineSelection.SelectionStart = startPosition;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                }
                else if (CompareSubLine(start) > 0)
                {
                    if (CompareSubLine(end) < 0)
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = CharWidths.Sum();
                        if (lineSelection.SelectionEnd < 1.0f)
                            lineSelection.SelectionEnd = 5.0f; //TODO get width of ' '

                        lineSelection.Selected = true;
                    }
                    else if (IsSameSubLine(end, out endPosition))
                    {
                        lineSelection.SelectionStart = 0.0f;
                        lineSelection.SelectionEnd = endPosition;
                        lineSelection.Selected = true;
                    }
                }
            }
            
            
            if (IsSameSubLine(caret.AnchorPosition, out float anchorPosition))
            {
                lineSelection.HasCaretAnchor = true;
                lineSelection.CaretAnchorPosition = anchorPosition;
            }
            
            if (IsSameSubLine(caret.Position, out float caretPosition))
            {
                lineSelection.HasCaret = true;
                lineSelection.CaretPosition = caretPosition;
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

    public bool HasCaretAnchor;
    public float CaretAnchorPosition;
    public bool HasCaret;
    public float CaretPosition;
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