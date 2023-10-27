using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class RowManager
{
    public List<Row> Rows => RowsCache.Get();
    public readonly Cache<List<Row>> RowsCache;
    
    // buffer to avoid GC
    private readonly List<char> _buffers = new();
    
    public RowManager()
    {
        RowsCache = new Cache<List<Row>>(new List<Row>(), UpdateRows);
    }
    
    private List<Row> UpdateRows(List<Row> rows)
    {
        rows.Clear();

        int foldingCount = 0;
        foreach (Line line in Singleton.LineManager.Lines)
        {
            bool visible = true;

            line.Folding = Folding.None;
            foreach (Folding folding in Singleton.FoldingManager.FoldingList)
            {
                if (folding.End == line.Index)
                {
                    foldingCount--;
                }

                if (folding.Start < line.Index && line.Index < folding.End)
                {
                    if (folding.Folded)
                    {
                        visible = (0 == foldingCount);
                        break;
                    }
                }

                if (folding.Start == line.Index)
                {
                    line.Folding = folding;
                    foldingCount++;
                }
            }

            if (visible)
            {
                for (int i = 0; i < line.LineSubs.Count; i++)
                {
                    LineSub lineSub = line.LineSubs[i];
                    float indentWidth = i == 0 ? 0.0f : line.GetIndentWidth();
                    
                    Row row = new Row(indentWidth, lineSub);

                    float startPosX = 0.0f;
                    float currPosX = 0.0f;
                    
                    _buffers.Clear();
                    float buffersWidth = 0.0f;

                    ColorStyle renderGroupColor = ColorStyle.None;
                    for (int j = 0; j < lineSub.Chars.Count; j++)
                    {
                        char c = lineSub.Chars[j];

                        ColorStyle charColor = line.Colors[lineSub.LineCoordinates.CharIndex + j];
                        float charWidth = lineSub.CharWidths[j];

                        if (j == 0)
                        {
                            renderGroupColor = charColor;
                        }
                        
                        if (renderGroupColor != charColor) // need new render group
                        {
                            row.TextBlockRenders.Add(new()
                            {
                                Text = String.Concat(_buffers),
                                ColorStyle = renderGroupColor,
                                PosX = startPosX
                            });

                            startPosX += buffersWidth;
                            
                            _buffers.Clear();
                            buffersWidth = 0.0f;
                            
                            renderGroupColor = charColor;
                        }
                        
                        _buffers.Add(c);
                        buffersWidth += charWidth;

                        if (Singleton.TextBox.ShowingWhitespace && char.IsWhiteSpace(c))
                        {
                            row.WhiteSpaceRenders.Add(new() { C = c, PosX = currPosX });
                        }
                        currPosX += charWidth;
                    }

                    // Add remains
                    if (_buffers.Count > 0)
                    {
                        row.TextBlockRenders.Add(new()
                        {
                            Text = String.Concat(_buffers),
                            ColorStyle = renderGroupColor,
                            PosX = startPosX
                        });
                    }
                    rows.Add(row);
                }
            }
        }

        return rows;
    }
}