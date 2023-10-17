using System.Runtime.InteropServices;
using Bell.Languages;

namespace Bell.Data;

public class Line
{
    private TextBox _textBox;

    public int Index = 0;

    public List<char> Chars = new();
    private List<char> _buffers = new();

    public string String => StringCache.Get();
    public readonly Cache<string> StringCache;

    public Dictionary<int, ColorStyle> Colors => ColorsCache.Get();
    public readonly Cache<Dictionary<int, ColorStyle>> ColorsCache;

    public bool Foldable => FoldableCache.Get();
    public readonly Cache<bool> FoldableCache;

    public HashSet<int> Cutoffs => CutoffsCache.Get();
    public readonly Cache<HashSet<int>> CutoffsCache;

    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;

    public bool Visible = true;
    public bool Folded = false;

    public Line(TextBox textBox, int index)
    {
        _textBox = textBox;
        Index = index;

        ColorsCache = new(new(), UpdateColors);
        CutoffsCache = new(new(), UpdateCutoff);
        FoldableCache = new(false, UpdateFoldable);
        StringCache = new(string.Empty, UpdateString);
        LineRendersCache = new(new List<LineRender>(), UpdateLineRenders);
    }

    public void SetString(string line)
    {
        Chars.Clear();
        Chars.AddRange(line);

        ColorsCache.SetDirty();
        CutoffsCache.SetDirty();
        FoldableCache.SetDirty();
        StringCache.SetDirty();
        LineRendersCache.SetDirty();
    }

    private Dictionary<int, ColorStyle> UpdateColors(Dictionary<int, ColorStyle> colors)
    {
        colors.Clear();
        for (int i = 0; i < Chars.Count; i++)
        {
            if (char.IsLower(Chars[i]))
            {
                colors[i] = _textBox.Theme.BlockCommentFontColor;
            }
            else if (false == char.IsAscii(Chars[i]))
            {
                colors[i] = _textBox.Theme.LineCommentFontColor;
            }
        }

        return colors;
    }

    private HashSet<int> UpdateCutoff(HashSet<int> cutoffs)
    {
        cutoffs.Clear();
        if (WrapMode.None == _textBox.WrapMode)
            return cutoffs;

        var lineWidth = _textBox.PageSize.X - _textBox.LineNumberWidth - _textBox.FoldWidth;
        if (lineWidth < 1.0f)
            return cutoffs;

        float widthAccumulated = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            widthAccumulated += _textBox.GetFontWidth(Chars[i]);
            if (widthAccumulated + _textBox.GetFontReferenceWidth() > lineWidth)
            {
                if (_textBox.WrapMode == WrapMode.BreakWord)
                {
                    cutoffs.Add(i);
                    widthAccumulated = 0;
                }
                else if (_textBox.WrapMode == WrapMode.Word)
                {
                    // go back to the start of word
                    float backWidth = 0.0f;
                    while (i > 0)
                    {
                        if (char.IsWhiteSpace(Chars[i]))
                            break;

                        backWidth += _textBox.GetFontWidth(Chars[i]);
                        if (backWidth + _textBox.GetFontReferenceWidth() * 10 > lineWidth)
                            break; // Give up on word wrap. break word.

                        i--;
                    }

                    cutoffs.Add(i);
                    widthAccumulated = 0;
                }
            }
        }

        return cutoffs;
    }

    private bool UpdateFoldable(bool _)
    {
        var trimmedString = String.TrimStart();
        foreach (ValueTuple<string, string> folding in _textBox.Language.Foldings)
        {
            if (trimmedString.StartsWith(folding.Item1))
                return true;
        }

        return false;
    }

    private string UpdateString(string _)
    {
        _textBox.StringBuilder.Clear();
        _textBox.StringBuilder.Append(CollectionsMarshal.AsSpan(Chars));
        return _textBox.StringBuilder.ToString();
    }
    
    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        float tabWidth = _textBox.CountTabStart(String) * _textBox.GetTabRenderSize(); // TODO Cache
        
        int wrapIndex = 0;
        LineRender lineRender = new LineRender(Index, wrapIndex, 0.0f);

        bool isFirstCharInLine = true;
        ColorStyle renderGroupColor = _textBox.Theme.DefaultFontColor;

        _buffers.Clear();
        float bufferWidth = 0.0f;
        float posX = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            char c = Chars[i];
            float cWidth = _textBox.GetFontWidth(c);
            
            lineRender.CharWidths.Add(cWidth);
            
            if (isFirstCharInLine)
            {
                if (Colors.TryGetValue(i, out var firstColor))
                {
                    renderGroupColor = firstColor;
                }

                _buffers.Add(c);
                bufferWidth += cWidth;

                isFirstCharInLine = false;
                continue;
            }

            if (false == Colors.TryGetValue(i, out ColorStyle color))
            {
                color = _textBox.Theme.DefaultFontColor;
            }

            if (renderGroupColor != color) // need new render group
            {
                lineRender.TextBlockRenders.Add(new()
                    { Text = String.Concat(_buffers), ColorStyle = renderGroupColor, Width = bufferWidth, PosX = posX });
                posX += bufferWidth;

                renderGroupColor = color;
                _buffers.Clear();
                bufferWidth = 0.0f;
            }

            _buffers.Add(c);
            bufferWidth += cWidth;

            if (Cutoffs.Contains(i)) // need new line
            {
                lineRender.TextBlockRenders.Add(new()
                    { Text = String.Concat(_buffers), ColorStyle = renderGroupColor, Width = bufferWidth, PosX = posX });
                lineRenders.Add(lineRender);
                
                wrapIndex++;
                lineRender = new LineRender(Index, wrapIndex, tabWidth);

                isFirstCharInLine = true;
                renderGroupColor = _textBox.Theme.DefaultFontColor;
                _buffers.Clear();
                
                posX = 0.0f;
                bufferWidth = 0.0f;
            }
        }

        // Add remains
        if (_buffers.Count > 0 || wrapIndex == 0)
            lineRender.TextBlockRenders.Add(new()
                { Text = String.Concat(_buffers), ColorStyle = renderGroupColor, Width = bufferWidth, PosX = posX });

        if (lineRender.TextBlockRenders.Count > 0)
            lineRenders.Add(lineRender);

        return lineRenders;
    }
}