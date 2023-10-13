﻿using System.Runtime.InteropServices;
using Bell.Languages;
using Bell.Render;

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

    public int RenderCount => Visible ? LineRenders.Count : 0;

    public Line(TextBox textBox)
    {
        _textBox = textBox;

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

        var lineWidth = _textBox.PageWidth - _textBox.LineNumberWidth - _textBox.FoldWidth;
        if (lineWidth < 1.0f)
            return cutoffs;
        
        float widthAccumulated = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            widthAccumulated += _textBox.FontSizeManager.GetFontWidth(Chars[i]);
            if (widthAccumulated + _textBox.FontSizeManager.GetFontReferenceWidth() > lineWidth)
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

                        backWidth += _textBox.FontSizeManager.GetFontWidth(Chars[i]);
                        if (backWidth + _textBox.FontSizeManager.GetFontReferenceWidth() * 10 > lineWidth)
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

    public LineRender GetLineRender(int renderIndex)
    {
        return LineRenders.Count <= renderIndex ? LineRender.NullLineRender : LineRenders[renderIndex];
    }

    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();

        LineRender lineRender = LineRender.Create();

        bool isFirstCharInLine = true;
        ColorStyle groupColor = _textBox.Theme.DefaultFontColor;

        _buffers.Clear();
        float bufferWidth = 0.0f;
        int wrapIndex = 0;

        for (int i = 0; i < Chars.Count; i++)
        {
            if (isFirstCharInLine)
            {
                if (Colors.TryGetValue(i, out var firstColor))
                {
                    groupColor = firstColor;
                }

                _buffers.Add(Chars[i]);
                bufferWidth += _textBox.FontSizeManager.GetFontWidth(Chars[i]);

                isFirstCharInLine = false;
                continue;
            }

            if (false == Colors.TryGetValue(i, out ColorStyle color))
            {
                color = _textBox.Theme.DefaultFontColor;
            }

            if (groupColor != color) // need new group
            {
                lineRender.TextBlockRenders.Add(new()
                    { Text = String.Concat(_buffers), ColorStyle = groupColor, Width = bufferWidth });

                groupColor = color;
                _buffers.Clear();
                bufferWidth = 0.0f;
            }

            _buffers.Add(Chars[i]);
            bufferWidth += _textBox.FontSizeManager.GetFontWidth(Chars[i]);

            if (Cutoffs.Contains(i)) // need new line
            {
                lineRender.TextBlockRenders.Add(new()
                    { Text = String.Concat(_buffers), ColorStyle = groupColor, Width = bufferWidth });
                lineRenders.Add(lineRender);
                wrapIndex++;

                lineRender = LineRender.Create();

                isFirstCharInLine = true;
                groupColor = _textBox.Theme.DefaultFontColor;
                _buffers.Clear();
                bufferWidth = 0.0f;
            }
        }

        // Add remains
        if (_buffers.Count > 0 || wrapIndex == 0)
            lineRender.TextBlockRenders.Add(new()
                { Text = String.Concat(_buffers), ColorStyle = groupColor, Width = bufferWidth });

        if (lineRender.TextBlockRenders.Count > 0)
            lineRenders.Add(lineRender);

        return lineRenders;
    }
}