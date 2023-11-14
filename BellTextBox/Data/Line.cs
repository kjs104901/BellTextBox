﻿using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Bell.Languages;
using Bell.Utils;

namespace Bell.Data;

internal class Line
{
    internal int Index;

    internal int CharsCount => _chars.Count;
    
    private readonly List<char> _chars = new();

    internal Folding Folding = Folding.None;

    internal string String => _stringCache.Get();
    private readonly Cache<string> _stringCache;

    internal List<ColorStyle> Colors => _colorsCache.Get();
    private readonly Cache<List<ColorStyle>> _colorsCache;

    private HashSet<int> Cutoffs => _cutoffsCache.Get();
    private readonly Cache<HashSet<int>> _cutoffsCache;

    internal List<LineSub> LineSubs => _lineSubsCache.Get();
    private readonly Cache<List<LineSub>> _lineSubsCache;

    private readonly List<Language.Token> _oldTokens = new();
    internal readonly List<Language.Token> Tokens = new();
    
    internal readonly List<ValueTuple<int, int>> CommentRanges = new();
    internal readonly List<ValueTuple<int, int>> StringRanges = new();
    
    // buffer to avoid GC
    private readonly StringBuilder _sb = new();

    internal static readonly Line None = new(0);

    internal Line(int index)
    {
        Index = index;
        
        _colorsCache = new("Colors", new(), UpdateColors, updateIntervalMs: 300);
        _cutoffsCache = new("Cutoffs", new(), UpdateCutoff);
        _stringCache = new("String", string.Empty, UpdateString);
        _lineSubsCache = new("Line Subs", new List<LineSub>(), UpdateLineSubs);
    }
    
    internal void ChangeLineIndex(int newIndex)
    {
        if (Index == newIndex)
            return;
        
        Index = newIndex;
        foreach (LineSub lineSub in LineSubs)
        {
            lineSub.Coordinates.LineIndex = newIndex;
        }
    }
    
    internal void InsertChars(int charIndex, char[] chars)
    {
        _chars.InsertRange(charIndex, chars);
        Colors.InsertRange(charIndex, new ColorStyle[chars.Length]);
        
        SetCharsDirty();
        SetCutoffsDirty();
        UpdateTokens();
        
        LineManager.Unfold(Index);
    }
    
    internal char[] RemoveChars(int charIndex, int count)
    {
        var removed = _chars.GetRange(charIndex, count).ToArray();
        _chars.RemoveRange(charIndex, count);
        
        if (Colors.Count > charIndex + count)
            Colors.RemoveRange(charIndex, count);
        else
            Colors.RemoveRange(charIndex, Colors.Count - charIndex);
        
        SetCharsDirty();
        SetCutoffsDirty();
        UpdateTokens();
        
        LineManager.Unfold(Index);
        
        return removed;
    }

    internal void SetCharsDirty()
    {
        _colorsCache.SetDirty();
        _stringCache.SetDirty();
    }
    
    internal void SetCutoffsDirty()
    {
        _cutoffsCache.SetDirty();
        _lineSubsCache.SetDirty();
    }

    private List<ColorStyle> UpdateColors(List<ColorStyle> colors)
    {
        colors.Clear();
        if (Singleton.TextBox.SyntaxHighlightEnabled == false)
            return colors;
        
        if (_chars.Count > TextBox.SyntaxGiveUpThreshold)
            return colors;
        
        colors.AddRange(new ColorStyle[_chars.Count]);

        foreach (var kv in Singleton.TextBox.Language.PatternsStyle)
        {
            Regex regex = kv.Key;
            ColorStyle colorStyle = kv.Value;
            
            foreach (Match match in regex.Matches(String))
            {
                for (int i = match.Index; i < match.Index + match.Length && i < colors.Count; i++)
                {
                    colors[i] = colorStyle;
                }
            }
        }

        foreach (var range in CommentRanges)
        {
            for (int i = range.Item1; i < range.Item2 && i < colors.Count; i++)
            {
                colors[i] = Singleton.TextBox.Language.CommentStyle;
            }
        }

        foreach (var range in StringRanges)
        {
            for (int i = range.Item1; i < range.Item2 && i < colors.Count; i++)
            {
                colors[i] = Singleton.TextBox.Language.StringStyle;
            }
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

        for (int i = 0; i < _chars.Count; i++)
        {
            widthAccumulated += FontManager.GetFontWidth(_chars[i]);
            if (widthAccumulated + FontManager.GetFontReferenceWidth() > lineWidth)
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
                        if (char.IsWhiteSpace(_chars[i]))
                            break;

                        backWidth += FontManager.GetFontWidth(_chars[i]);
                        if (backWidth + FontManager.GetFontReferenceWidth() * 10 > lineWidth)
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
        _sb.Append(CollectionsMarshal.AsSpan(_chars));
        return _sb.ToString();
    }

    private List<LineSub> UpdateLineSubs(List<LineSub> lineSubs)
    {
        lineSubs.Clear();

        int lineSubIndex = 0;
        LineSub lineSub = new LineSub(Index, 0, lineSubIndex, 0.0f);

        for (int i = 0; i < _chars.Count; i++)
        {
            char c = _chars[i];
            float cWidth = FontManager.GetFontWidth(c);

            lineSub.Chars.Add(c);
            lineSub.CharWidths.Add(cWidth);

            if (Cutoffs.Contains(i)) // need new line
            {
                lineSubs.Add(lineSub);

                lineSubIndex++;
                lineSub = new LineSub(Index, i + 1, lineSubIndex, GetIndentWidth());
            }
        }

        lineSubs.Add(lineSub);
        return lineSubs;
    }

    private void UpdateTokens()
    {
        Tokens.Clear();

        for (int i = 0; i < String.Length; i++)
        {
            if (Singleton.TextBox.Language.FindMatching(String,
                    Index, i, out Language.Token matchedToken))
            {
                Tokens.Add(matchedToken);
                i += matchedToken.TokenString.Length;
                i--; // Ignore i++ in for loop
            }
        }
        
        if (false == _oldTokens.SequenceEqual(Tokens))
        {
            _oldTokens.Clear();
            _oldTokens.AddRange(Tokens);
            LineManager.SetLanguageDirty();
        }
    }
    
    internal bool GetLineSub(int charIndex, out LineSub foundLineSub)
    {
        foundLineSub = LineSub.None;

        foreach (LineSub lineSub in LineSubs)
        {
            if (lineSub.Coordinates.CharIndex <= charIndex &&
                charIndex <= lineSub.Coordinates.CharIndex + lineSub.Chars.Count + 1)
            {
                foundLineSub = lineSub;
                return true;
            }
        }

        Logger.Error($"GetLineSub failed to find. LineIndex: {Index}, charIndex: {charIndex}");
        foundLineSub = LineSubs[0];
        return false;
    }

    private float GetIndentWidth()
    {
        if (Singleton.TextBox.WordWrapIndent)
            return Singleton.TextBox.CountTabStart(String) * Singleton.TextBox.GetTabRenderSize();
        return 0.0f;
    }

    internal ColorStyle GetColorStyle(int charIndex)
    {
        ColorStyle charColor = Singleton.TextBox.Language.DefaultStyle;
        if (Colors.Count > charIndex)
            charColor = Colors[charIndex];
        if (charColor == ColorStyle.None)
            charColor = Singleton.TextBox.Language.DefaultStyle;
        return charColor;
    }

    public bool GetWordStart(int charIndex, out int search)
    {
        search = charIndex;
        if (charIndex < 0 || charIndex >= _chars.Count)
            return false;

        while (search >= 0)
        {
            if (_chars[search] == ' ')
            {
                search++;
                break;
            }
            search--;
        }
        return true;
    }

    public bool GetWordEnd(int charIndex, out int search)
    {
        search = charIndex;
        if (charIndex < 0 || charIndex >= _chars.Count)
            return false;

        while (search < _chars.Count)
        {
            if (_chars[search] == ' ')
                break;
            search++;
        }
        return true;
    }

    public string GetSubString(int startCharIndex, int endCharIndex)
    {
        if (startCharIndex > endCharIndex)
            return string.Empty;
        
        if (startCharIndex < 0)
            startCharIndex = 0;
        
        if (endCharIndex >= _chars.Count)
            endCharIndex = _chars.Count - 1;

        return String.Substring(startCharIndex, endCharIndex - startCharIndex + 1);
    }
}