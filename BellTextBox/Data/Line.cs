using System.Runtime.InteropServices;
using System.Text;
using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

internal class Line
{
    internal int Index;

    internal readonly List<char> Chars = new();

    internal Folding Folding = Folding.None;

    internal string String => _stringCache.Get();
    private readonly Cache<string> _stringCache;

    internal List<ColorStyle> Colors => _colorsCache.Get();
    private readonly Cache<List<ColorStyle>> _colorsCache;

    private HashSet<int> Cutoffs => _cutoffsCache.Get();
    private readonly Cache<HashSet<int>> _cutoffsCache;

    internal List<LineSub> LineSubs => _lineSubsCache.Get();
    private readonly Cache<List<LineSub>> _lineSubsCache;

    // buffer to avoid GC
    private readonly StringBuilder _sb = new();

    internal static readonly Line None = new(0, Array.Empty<char>());

    internal Line(int index, char[]? initialChars = null)
    {
        Index = index;
        if (initialChars != null)
            Chars.AddRange(initialChars);

        _colorsCache = new("Colors", new(), UpdateColors);
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

    internal void SetCharsDirty()
    {
        _colorsCache.SetDirty();
        _cutoffsCache.SetDirty();
        _stringCache.SetDirty();
        _lineSubsCache.SetDirty();
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
            widthAccumulated += FontManager.GetFontWidth(Chars[i]);
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
                        if (char.IsWhiteSpace(Chars[i]))
                            break;

                        backWidth += FontManager.GetFontWidth(Chars[i]);
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
        _sb.Append(CollectionsMarshal.AsSpan(Chars));
        return _sb.ToString();
    }

    private List<LineSub> UpdateLineSubs(List<LineSub> lineSubs)
    {
        lineSubs.Clear();

        int lineSubIndex = 0;
        LineSub lineSub = new LineSub(Index, 0, lineSubIndex, 0.0f);

        for (int i = 0; i < Chars.Count; i++)
        {
            char c = Chars[i];
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

    internal int CountSubstrings(string findText)
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
}