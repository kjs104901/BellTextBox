using System.Runtime.InteropServices;
using Bell.Languages;
using Bell.Render;

namespace Bell.Data;

public class Line
{
    private TextBox _textBox;

    public int Index = 0;

    private List<char> _chars = new();
    private List<char> _buffers = new();

    public string String => StringCache.Get();
    public readonly Cache<string> StringCache;

    public Dictionary<int, FontStyle> Styles => StylesCache.Get();
    public readonly Cache<Dictionary<int, FontStyle>> StylesCache;

    public bool Foldable => FoldableCache.Get();
    public readonly Cache<bool> FoldableCache;

    public HashSet<int> Cutoffs => CutoffsCache.Get();
    public readonly Cache<HashSet<int>> CutoffsCache;

    public List<LineRender> LineRenders => LineRendersCache.Get();
    public readonly Cache<List<LineRender>> LineRendersCache;

    public bool Visible = true;
    public bool Folded = false;

    public int RenderCount => Visible ? LineRenders.Count : 0;

    public Line(TextBox textBox)
    {
        _textBox = textBox;

        StylesCache = new(new(), UpdateStyles);
        CutoffsCache = new(new(), UpdateCutoff);
        FoldableCache = new(false, UpdateFoldable);
        StringCache = new(string.Empty, UpdateString);
        LineRendersCache = new(new List<LineRender>(), UpdateLineRenders);
    }

    public void SetString(string line)
    {
        _chars.Clear();
        _chars.AddRange(line);

        StylesCache.SetDirty();
        CutoffsCache.SetDirty();
        FoldableCache.SetDirty();
        StringCache.SetDirty();
        LineRendersCache.SetDirty();
    }

    private Dictionary<int, FontStyle> UpdateStyles(Dictionary<int, FontStyle> styles)
    {
        styles.Clear();
        for (int i = 0; i < _chars.Count; i++)
        {
            if (char.IsLower(_chars[i]))
            {
                styles[i] = FontStyle.BlockCommentFontStyle;
            }
            else if (false == char.IsAscii(_chars[i]))
            {
                styles[i] = FontStyle.LineCommentFontStyle;
            }
        }

        return styles;
    }

    private HashSet<int> UpdateCutoff(HashSet<int> cutoffs)
    {
        cutoffs.Clear();
        if (_textBox.WrapMode == WrapMode.Word || _textBox.WrapMode == WrapMode.BreakWord)
        {
            float widthAccumulated = 0.0f;
            for (int i = 0; i < _chars.Count; i++)
            {
                widthAccumulated += _textBox.FontSizeManager.GetFontWidth(_chars[i]);
                if (widthAccumulated + _textBox.FontSizeManager.GetFontReferenceWidth() >
                    500 - _textBox.LineNumberWidth - _textBox.FoldWidth) // TODO handle width
                {
                    if (_textBox.WrapMode == WrapMode.BreakWord)
                    {
                        cutoffs.Add(i);
                        widthAccumulated = 0;
                    }
                    else if (_textBox.WrapMode == WrapMode.Word)
                    {
                        // go back to the start of word
                        while (i > 0 && false == char.IsWhiteSpace(_chars[i]))
                            i--;
                        
                        cutoffs.Add(i);
                        widthAccumulated = 0;
                    }
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
        _textBox.StringBuilder.Append(CollectionsMarshal.AsSpan(_chars));
        return _textBox.StringBuilder.ToString();
    }

    public LineRender GetLineRender(int renderIndex)
    {
        return LineRenders.Count <= renderIndex ? LineRender.NullLineRender : LineRenders[renderIndex];
    }

    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        LineRender lineRender = LineRender.Create();

        bool isFirstCharInLine = true;
        FontStyle groupStyle = FontStyle.DefaultFontStyle;

        _buffers.Clear();
        float bufferWidth = 0.0f;
        int wrapIndex = 0;

        for (int i = 0; i < _chars.Count; i++)
        {
            if (isFirstCharInLine)
            {
                if (Styles.TryGetValue(i, out var firstStyle))
                {
                    groupStyle = firstStyle;
                }

                _buffers.Add(_chars[i]);
                bufferWidth += _textBox.FontSizeManager.GetFontWidth(_chars[i]);

                isFirstCharInLine = false;
                continue;
            }

            if (false == Styles.TryGetValue(i, out var charStyle))
            {
                charStyle = FontStyle.DefaultFontStyle;
            }

            if (groupStyle != charStyle) // need new group
            {
                lineRender.TextBlockRenders.Add(new()
                    { Text = String.Concat(_buffers), FontStyle = groupStyle, Width = bufferWidth });

                groupStyle = charStyle;
                _buffers.Clear();
                bufferWidth = 0.0f;
            }

            _buffers.Add(_chars[i]);
            bufferWidth += _textBox.FontSizeManager.GetFontWidth(_chars[i]);

            if (Cutoffs.Contains(i)) // need new line
            {
                lineRender.TextBlockRenders.Add(new()
                    { Text = String.Concat(_buffers), FontStyle = groupStyle, Width = bufferWidth });
                lineRenders.Add(lineRender);
                wrapIndex++;

                lineRender = LineRender.Create();

                isFirstCharInLine = true;
                groupStyle = FontStyle.DefaultFontStyle;
                _buffers.Clear();
                bufferWidth = 0.0f;
            }
        }

        // Add remains
        if (_buffers.Count > 0 || wrapIndex == 0)
            lineRender.TextBlockRenders.Add(new()
                { Text = String.Concat(_buffers), FontStyle = groupStyle, Width = bufferWidth });

        if (lineRender.TextBlockRenders.Count > 0)
            lineRenders.Add(lineRender);

        return lineRenders;
    }
}