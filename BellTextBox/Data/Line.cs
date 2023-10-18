using System.Runtime.InteropServices;
using Bell.Languages;
using Bell.Utils;

namespace Bell.Data;

public class Line
{
    private TextBox _textBox;

    public int Index = 0;

    public List<char> Chars = new();
    private List<char> _buffers = new();

    public string String => StringCache.Get();
    public readonly Cache<string> StringCache;

    public List<ColorStyle> Colors => ColorsCache.Get();
    public readonly Cache<List<ColorStyle>> ColorsCache;

    public bool Foldable => FoldableCache.Get();
    public readonly Cache<bool> FoldableCache;

    public HashSet<int> Cutoffs => CutoffsCache.Get();
    public readonly Cache<HashSet<int>> CutoffsCache;

    public List<SubLine> SubLines => SubLinesCache.Get();
    public readonly Cache<List<SubLine>> SubLinesCache;

    public Line(TextBox textBox, int index)
    {
        _textBox = textBox;
        Index = index;

        ColorsCache = new(new(), UpdateColors);
        CutoffsCache = new(new(), UpdateCutoff);
        FoldableCache = new(false, UpdateFoldable);
        StringCache = new(string.Empty, UpdateString);
        SubLinesCache = new(new List<SubLine>(), UpdateSubLines);
    }

    public void SetString(string line)
    {
        Chars.Clear();
        Chars.AddRange(line);

        ColorsCache.SetDirty();
        CutoffsCache.SetDirty();
        FoldableCache.SetDirty();
        StringCache.SetDirty();
        SubLinesCache.SetDirty();
    }

    private List<ColorStyle> UpdateColors(List<ColorStyle> colors)
    {
        colors.Clear();
        if (_textBox.SyntaxHighlightEnabled == false)
            return colors;

        for (int i = 0; i < Chars.Count; i++)
        {
            ColorStyle colorStyle;
            if (char.IsLower(Chars[i]))
            {
                colorStyle = _textBox.Theme.BlockCommentFontColor;
            }
            else if (false == char.IsAscii(Chars[i]))
            {
                colorStyle = _textBox.Theme.LineCommentFontColor;
            }
            else
            {
                colorStyle = _textBox.Theme.DefaultFontColor;
            }
            colors.Add(colorStyle);
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

    private List<SubLine> UpdateSubLines(List<SubLine> subLines)
    {
        subLines.Clear();

        float tabWidth = _textBox.CountTabStart(String) * _textBox.GetTabRenderSize(); // TODO Cache

        int subIndex = 0;
        SubLine subLine = new SubLine(Index, subIndex, 0.0f);

        bool isFirstCharInLine = true;
        ColorStyle renderGroupColor = _textBox.Theme.DefaultFontColor;

        _buffers.Clear();
        float bufferWidth = 0.0f;
        float posX = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            char c = Chars[i];
            float cWidth = _textBox.GetFontWidth(c);

            subLine.CharWidths.Add(cWidth);
            
            if (_textBox.ShowingWhitespace && char.IsWhiteSpace(c))
            {
                subLine.WhiteSpaceRenders.Add(new() { C = c, PosX = posX + bufferWidth });
            }
            
            if (isFirstCharInLine)
            {
                renderGroupColor = Colors[i];
                
                _buffers.Add(c);
                bufferWidth += cWidth;

                isFirstCharInLine = false;
                continue;
            }
            
            if (renderGroupColor != Colors[i]) // need new render group
            {
                subLine.TextBlockRenders.Add(new()
                {
                    Text = String.Concat(_buffers), ColorStyle = renderGroupColor, Width = bufferWidth, PosX = posX
                });
                posX += bufferWidth;

                renderGroupColor = Colors[i];
                _buffers.Clear();
                bufferWidth = 0.0f;
            }

            _buffers.Add(c);
            bufferWidth += cWidth;

            if (Cutoffs.Contains(i)) // need new line
            {
                subLine.TextBlockRenders.Add(new()
                {
                    Text = String.Concat(_buffers), ColorStyle = renderGroupColor, Width = bufferWidth, PosX = posX
                });
                subLines.Add(subLine);

                subIndex++;
                subLine = new SubLine(Index, subIndex, tabWidth);

                isFirstCharInLine = true;
                renderGroupColor = _textBox.Theme.DefaultFontColor;
                _buffers.Clear();

                posX = 0.0f;
                bufferWidth = 0.0f;
            }
        }

        // Add remains
        if (_buffers.Count > 0 || subIndex == 0)
            subLine.TextBlockRenders.Add(new()
                { Text = String.Concat(_buffers), ColorStyle = renderGroupColor, Width = bufferWidth, PosX = posX });

        if (subLine.TextBlockRenders.Count > 0)
            subLines.Add(subLine);

        return subLines;
    }

    public int CountSubstrings(string findText)
    {
        int count = 0;
        
        int textLength = Chars.Count;
        int findTextLength = findText.Length;

        for (int i = 0; i <= textLength - findTextLength; i++)
        {
            bool isMatch = true;
            for (int j = 0; j < findTextLength; j++)
            {
                if (Chars[i + j] != findText[j])
                {
                    isMatch = false;
                    break;
                }
            }
            if (isMatch)
            {
                count++;
                i += findTextLength - 1;  // Skip the length of substring to avoid overlapping counts
            }
        }
        return count;
    }
}