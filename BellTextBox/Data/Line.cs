﻿using System.Runtime.InteropServices;
using System.Text;
using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

public class Line
{
    public int Index = 0;

    public readonly List<char> Chars = new();
    
    public Folding Folding = Folding.None;
    
    public string String => StringCache.Get();
    public readonly Cache<string> StringCache;

    public List<ColorStyle> Colors => ColorsCache.Get();
    public readonly Cache<List<ColorStyle>> ColorsCache;
    
    public HashSet<int> Cutoffs => CutoffsCache.Get();
    public readonly Cache<HashSet<int>> CutoffsCache;

    public List<SubLine> SubLines => SubLinesCache.Get();
    public readonly Cache<List<SubLine>> SubLinesCache;
    
    // buffer to avoid GC
    private readonly StringBuilder _sb = new();
    private readonly List<char> _buffers = new();
    
    public static readonly Line Empty = new(0, Array.Empty<char>());

    public Line(int index, char[] initialChars)
    {
        Index = index;
        Chars.AddRange(initialChars);
        
        ColorsCache = new(new(), UpdateColors);
        CutoffsCache = new(new(), UpdateCutoff);
        StringCache = new(string.Empty, UpdateString);
        SubLinesCache = new(new List<SubLine>(), UpdateSubLines);
    }

    public void SetCharsDirty()
    {
        ColorsCache.SetDirty();
        CutoffsCache.SetDirty();
        StringCache.SetDirty();
        SubLinesCache.SetDirty();
    }

    private List<ColorStyle> UpdateColors(List<ColorStyle> colors)
    {
        colors.Clear();
        if (ThreadLocal.TextBox.SyntaxHighlightEnabled == false)
            return colors;

        for (int i = 0; i < Chars.Count; i++)
        {
            ColorStyle colorStyle;
            if (char.IsLower(Chars[i]))
            {
                colorStyle = ThreadLocal.TextBox.Theme.BlockCommentFontColor;
            }
            else if (false == char.IsAscii(Chars[i]))
            {
                colorStyle = ThreadLocal.TextBox.Theme.LineCommentFontColor;
            }
            else
            {
                colorStyle = ThreadLocal.TextBox.Theme.DefaultFontColor;
            }
            colors.Add(colorStyle);
        }

        return colors;
    }

    private HashSet<int> UpdateCutoff(HashSet<int> cutoffs)
    {
        cutoffs.Clear();
        if (WrapMode.None == ThreadLocal.TextBox.WrapMode)
            return cutoffs;

        var lineWidth = ThreadLocal.TextBox.PageSize.X - ThreadLocal.TextBox.LineNumberWidth - ThreadLocal.TextBox.FoldWidth;
        if (lineWidth < 1.0f)
            return cutoffs;

        float widthAccumulated = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            widthAccumulated += ThreadLocal.FontManager.GetFontWidth(Chars[i]);
            if (widthAccumulated + ThreadLocal.FontManager.GetFontReferenceWidth() > lineWidth)
            {
                if (ThreadLocal.TextBox.WrapMode == WrapMode.BreakWord)
                {
                    cutoffs.Add(i);
                    widthAccumulated = 0;
                }
                else if (ThreadLocal.TextBox.WrapMode == WrapMode.Word)
                {
                    // go back to the start of word
                    float backWidth = 0.0f;
                    while (i > 0)
                    {
                        if (char.IsWhiteSpace(Chars[i]))
                            break;

                        backWidth += ThreadLocal.FontManager.GetFontWidth(Chars[i]);
                        if (backWidth + ThreadLocal.FontManager.GetFontReferenceWidth() * 10 > lineWidth)
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

    private string UpdateString(string _)
    {
        _sb.Clear();
        _sb.Append(CollectionsMarshal.AsSpan(Chars));
        return _sb.ToString();
    }

    private List<SubLine> UpdateSubLines(List<SubLine> subLines)
    {
        subLines.Clear();
        
        float wrapIndentWidth = 0.0f;
        if (ThreadLocal.TextBox.WordWrapIndent)
        {
            // TODO Cache
            wrapIndentWidth = ThreadLocal.TextBox.CountTabStart(String) * ThreadLocal.TextBox.GetTabRenderSize(); 
        }

        int wrapIndex = 0;
        SubLine subLine = new SubLine(this, 0, wrapIndex, 0.0f);

        bool isFirstCharInLine = true;
        ColorStyle renderGroupColor = ThreadLocal.TextBox.Theme.DefaultFontColor;

        _buffers.Clear();
        float bufferWidth = 0.0f;
        float posX = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            char c = Chars[i];
            float cWidth = ThreadLocal.FontManager.GetFontWidth(c);

            subLine.Chars.Add(c);
            subLine.CharWidths.Add(cWidth);
            
            if (ThreadLocal.TextBox.ShowingWhitespace && char.IsWhiteSpace(c))
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

                wrapIndex++;
                subLine = new SubLine(this, i, wrapIndex, wrapIndentWidth);

                isFirstCharInLine = true;
                renderGroupColor = ThreadLocal.TextBox.Theme.DefaultFontColor;
                _buffers.Clear();

                posX = 0.0f;
                bufferWidth = 0.0f;
            }
        }

        // Add remains
        if (_buffers.Count > 0 || wrapIndex == 0)
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

    public struct TextInfo
    {
        
    }
}