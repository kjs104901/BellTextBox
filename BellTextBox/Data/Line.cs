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

    public List<LineSub> LineSubs => LineSubsCache.Get();
    public readonly Cache<List<LineSub>> LineSubsCache;

    // buffer to avoid GC
    private readonly StringBuilder _sb = new();

    public static readonly Line Empty = new(0, Array.Empty<char>());

    public Line(int index, char[] initialChars)
    {
        Index = index;
        Chars.AddRange(initialChars);

        ColorsCache = new(new(), UpdateColors);
        CutoffsCache = new(new(), UpdateCutoff);
        StringCache = new(string.Empty, UpdateString);
        LineSubsCache = new(new List<LineSub>(), UpdateLineSubs);
    }

    public void SetCharsDirty()
    {
        ColorsCache.SetDirty();
        CutoffsCache.SetDirty();
        StringCache.SetDirty();
        LineSubsCache.SetDirty();
    }

    private List<ColorStyle> UpdateColors(List<ColorStyle> colors)
    {
        colors.Clear();
        if (Singleton.TextBox.SyntaxHighlightEnabled == false)
            return colors;

        for (int i = 0; i < Chars.Count; i++)
        {
            ColorStyle colorStyle;
            if (char.IsLower(Chars[i]))
            {
                colorStyle = Singleton.TextBox.Theme.BlockCommentFontColor;
            }
            else if (false == char.IsAscii(Chars[i]))
            {
                colorStyle = Singleton.TextBox.Theme.LineCommentFontColor;
            }
            else
            {
                colorStyle = Singleton.TextBox.Theme.DefaultFontColor;
            }

            colors.Add(colorStyle);
        }

        return colors;
    }

    private HashSet<int> UpdateCutoff(HashSet<int> cutoffs)
    {
        cutoffs.Clear();
        if (WrapMode.None == Singleton.TextBox.WrapMode)
            return cutoffs;

        var lineWidth = Singleton.TextBox.PageSize.X - Singleton.TextBox.LineNumberWidth - Singleton.TextBox.FoldWidth;
        if (lineWidth < 1.0f)
            return cutoffs;

        float widthAccumulated = 0.0f;

        for (int i = 0; i < Chars.Count; i++)
        {
            widthAccumulated += Singleton.FontManager.GetFontWidth(Chars[i]);
            if (widthAccumulated + Singleton.FontManager.GetFontReferenceWidth() > lineWidth)
            {
                if (Singleton.TextBox.WrapMode == WrapMode.BreakWord)
                {
                    cutoffs.Add(i);
                    widthAccumulated = 0;
                }
                else if (Singleton.TextBox.WrapMode == WrapMode.Word)
                {
                    // go back to the start of word
                    float backWidth = 0.0f;
                    while (i > 0)
                    {
                        if (char.IsWhiteSpace(Chars[i]))
                            break;

                        backWidth += Singleton.FontManager.GetFontWidth(Chars[i]);
                        if (backWidth + Singleton.FontManager.GetFontReferenceWidth() * 10 > lineWidth)
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

    private List<LineSub> UpdateLineSubs(List<LineSub> lineSubs)
    {
        lineSubs.Clear();

        int subIndexIndex = 0;
        LineSub lineSub = new LineSub(this, 0, subIndexIndex);

        for (int i = 0; i < Chars.Count; i++)
        {
            char c = Chars[i];
            float cWidth = Singleton.FontManager.GetFontWidth(c);

            lineSub.Chars.Add(c);
            lineSub.CharWidths.Add(cWidth);

            if (Cutoffs.Contains(i)) // need new line
            {
                lineSubs.Add(lineSub);

                subIndexIndex++;
                lineSub = new LineSub(this, i, subIndexIndex);
            }
        }

        lineSubs.Add(lineSub);
        return lineSubs;
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
                i += findTextLength - 1; // Skip the length of substring to avoid overlapping counts
            }
        }

        return count;
    }

    public LineSub GetLineSub(int charIndex)
    {
        foreach (LineSub lineSub in LineSubs)
        {
            // TODO FIXME 좌표에 문제있다. 중복 좌표 문제
            if (lineSub.LineCoordinates.CharIndex <= charIndex &&
                charIndex <= lineSub.LineCoordinates.CharIndex + lineSub.Chars.Count)
            {
                //Singleton.Logger.Info("GetLineSub charIndex: {charIndex}, indexCount: {indexCount}, lineSub.Chars.Count: {lineSub.Chars.Count}");
                return lineSub;
            }
        }

        Singleton.Logger.Error("GetLineSub failed to find. charIndex: {charIndex}, indexCount: {indexCount}");
        return LineSubs[0];
    }

    public float GetIndentWidth()
    {
        if (Singleton.TextBox.WordWrapIndent)
            return Singleton.TextBox.CountTabStart(String) * Singleton.TextBox.GetTabRenderSize();
        return 0.0f;
    }
}