﻿using Bell.Data;
using Bell.Languages;

namespace Bell;

public enum WrapMode
{
    None,
    Word,
    BreakWord
}

public enum EolMode
{
    CRLF,
    LF,
    CR
}

public enum TabMode
{
    Space,
    Tab
}

public partial class TextBox
{
    public bool AutoIndent { get; set; } = true; // TODO
    public bool AutoComplete { get; set; } = true;
    public bool Overwrite { get; set; } = false;
    public bool ReadOnly { get; set; } = false;
    public WrapMode WrapMode { get; set; } = WrapMode.Word;
    public bool WordWrapIndent { get; set; } = true;
    public TabMode TabMode = TabMode.Space;
    public int TabSize = 4;
    
    public EolMode EolMode = EolMode.LF;
    
    public bool SyntaxHighlighting { get; set; } = true;
    public bool SyntaxFolding { get; set; } = true;

    public bool SyntaxHighlightEnabled { get; set; } = true;
    public bool ShowingWhitespace { get; set; } = true;
    
    internal const int SyntaxGiveUpThreshold = 1000;
    
    public float LeadingHeight { get; set; } = 1.2f;
    
    private Language _language = Language.PlainText();
    public Language Language
    {
        get => _language;
        set
        {
            _language = value;
            LineManager.SetLanguageTokenDirty();
        }
    }

    public readonly List<string> AutoCompleteList = new();

    internal int CountTabStart(string line)
    {
        string tabString = GetTabString();

        int count = 0;
        while (line.StartsWith(tabString))
        {
            count++;
            line = line.Remove(0, tabString.Length);
        }
        return count;
    }

    internal string GetTabString()
    {
        if (TabMode.Space == TabMode)
            return new string(' ', TabSize); // TODO: cache this
        return "\t";
    }

    private string ReplaceTab(string text)
    {
        switch (TabMode)
        {
            case TabMode.Space:
                return text.Replace("\t", new string(' ', TabSize));
            case TabMode.Tab:
                return text.Replace(new string(' ', TabSize), "\t");
        }
        return text;
    }

    internal string ReplaceEol(string text)
    {
        return text.Replace("\r\n", "\n")
            .Replace("\r", "\n");
    }

    internal string GetEolString()
    {
        switch (EolMode)
        {
            case EolMode.CRLF:
                return "\r\n";
            case EolMode.LF:
                return "\n";
            case EolMode.CR:
                return "\r";
        }
        return "\n";
    }
}