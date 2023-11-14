﻿using Bell.Actions;
using Bell.Languages;
using Bell.Utils;

namespace Bell.Data;

// Interface
internal partial class LineManager
{
    internal static List<Line> Lines => Singleton.TextBox.LineManager._lines;
    internal static List<Folding> FoldingList => Singleton.TextBox.LineManager._foldingList;

    internal static bool GetLine(int lineIndex, out Line line) =>
        Singleton.TextBox.LineManager.GetLine_(lineIndex, out line);

    internal static bool GetLineSub(int lineIndex, int lineSubIndex, out LineSub lineSub) =>
        Singleton.TextBox.LineManager.GetLineSub_(lineIndex, lineSubIndex, out lineSub);

    internal static bool GetLineSub(Coordinates coordinates, out LineSub lineSub) =>
        Singleton.TextBox.LineManager.GetLineSub_(coordinates, out lineSub);

    internal static Line InsertLine(int lineIndex) =>
        Singleton.TextBox.LineManager.InsertLine_(lineIndex);

    internal static void RemoveLine(int removeLineIndex) =>
        Singleton.TextBox.LineManager.RemoveLine_(removeLineIndex);

    internal static void SetLanguageDirty() =>
        Singleton.TextBox.LineManager.SetLanguageDirty_();

    internal static void UpdateLanguage() =>
        Singleton.TextBox.LineManager.UpdateLanguage_();
    
    internal static void ShiftFoldingLine(int lineIndex, EditDirection direction) =>
        Singleton.TextBox.LineManager.ShiftFoldingLine_(lineIndex, direction);

    public static void Unfold(int lineIndex) =>
        Singleton.TextBox.LineManager.Unfold_(lineIndex);
}

// Implementation
internal partial class LineManager
{
    private readonly List<Line> _lines = new();

    private readonly List<Folding> _foldingList = new();
    private readonly List<Folding> _lineFoldingList = new();
    private readonly List<Folding> _foldedList = new();
    private readonly Dictionary<int, Stack<int>> _foldingStacks = new();

    private readonly Stack<Language.Token> _tokens = new();

    private bool _isLanguageDirty = true;
    private DateTime _languageUpdateTime = DateTime.Now;
    private const int LanguageUpdateInterval = 100;

    private bool GetLine_(int lineIndex, out Line line)
    {
        if (0 <= lineIndex && lineIndex < _lines.Count)
        {
            line = _lines[lineIndex];
            if (line.Index != lineIndex)
            {
                Logger.Error($"LineManager Line.Index != lineIndex: {line.Index} != {lineIndex}");
            }

            return true;
        }

        line = Line.None;
        return false;
    }

    private bool GetLineSub_(int lineIndex, int lineSubIndex, out LineSub lineSub)
    {
        lineSub = LineSub.None;
        if (false == GetLine(lineIndex, out Line line))
            return false;

        if (line.LineSubs.Count <= lineSubIndex)
            return false;

        lineSub = line.LineSubs[lineSubIndex];
        return true;
    }

    private bool GetLineSub_(Coordinates coordinates, out LineSub lineSub)
    {
        lineSub = LineSub.None;
        if (coordinates.LineSubIndex >= 0 &&
            GetLineSub(coordinates.LineIndex, coordinates.LineSubIndex, out lineSub))
        {
            return true;
        }

        return GetLine(coordinates.LineIndex, out Line line) && line.GetLineSub(coordinates.CharIndex, out lineSub);
    }

    private Line InsertLine_(int lineIndex)
    {
        Line newLine = new Line(lineIndex);
        _lines.Insert(lineIndex, newLine);

        // Update line index
        for (int i = lineIndex; i < _lines.Count; i++)
        {
            _lines[i].ChangeLineIndex(i);
        }

        return newLine;
    }

    private void RemoveLine_(int removeLineIndex)
    {
        _lines.RemoveAt(removeLineIndex);

        // Update line index
        for (int i = removeLineIndex; i < _lines.Count; i++)
        {
            _lines[i].ChangeLineIndex(i);
        }
    }

    private void SetLanguageDirty_()
    {
        _isLanguageDirty = true;
        _languageUpdateTime = DateTime.Now.AddMilliseconds(LanguageUpdateInterval);
    }

    private void UpdateLanguage_()
    {
        if (false == _isLanguageDirty || _languageUpdateTime > DateTime.Now)
            return;

        SaveFolded_();
        _foldingList.Clear();
        
        int foldingTypeCount = Singleton.TextBox.Language.Tokens[Language.TokenType.FoldingStart].Count;
        for (int i = 0; i < foldingTypeCount; i++)
        {
            if (_foldingStacks.ContainsKey(i) == false)
                _foldingStacks.TryAdd(i, new Stack<int>());
            _foldingStacks[i].Clear();
        }
        
        _tokens.Clear();
        foreach (Line line in _lines)
        {
            line.CommentRanges.Clear();
            line.StringRanges.Clear();
            
            _lineFoldingList.Clear();
            
            int commentStart = -1;
            int stringStart = -1;
            
            if (_tokens.TryPeek(out Language.Token prevTop))
            {
                if (prevTop.Type == Language.TokenType.BlockCommentStart)
                {
                    commentStart = 0;
                }
                else if (prevTop.Type == Language.TokenType.MultilineStringStart)
                {
                    stringStart = 0;
                }
            }

            foreach (Language.Token token in line.Tokens)
            {
                if (_tokens.TryPeek(out Language.Token top))
                {
                    // If the previous token is not finished, ignore the token with continue
                    if (top.Type == Language.TokenType.BlockCommentStart)
                    {
                        if (token.Type != Language.TokenType.BlockCommentEnd ||
                            token.TokenIndex != top.TokenIndex)
                            continue;

                        _tokens.Pop();
                        line.CommentRanges.Add(new ValueTuple<int, int>(commentStart, token.CharIndex + token.TokenString.Length));
                        commentStart = -1;
                        continue;
                    }

                    if (top.Type == Language.TokenType.MultilineStringStart)
                    {
                        if (token.Type != Language.TokenType.MultilineStringEnd ||
                            token.TokenIndex != top.TokenIndex)
                            continue;

                        _tokens.Pop();
                        line.StringRanges.Add(new ValueTuple<int, int>(stringStart, token.CharIndex + token.TokenString.Length));
                        stringStart = -1;
                        continue;
                    }

                    if (top.Type == Language.TokenType.String)
                    {
                        if (token.Type == Language.TokenType.String &&
                            token.TokenIndex == top.TokenIndex)
                        {
                            _tokens.Pop();
                            line.StringRanges.Add(new ValueTuple<int, int>(stringStart, token.CharIndex + token.TokenString.Length));
                            stringStart = -1;
                        }
                        continue;
                    }

                    if (top.Type == Language.TokenType.LineComment)
                    {
                        continue;
                    }
                }

                _tokens.Push(token);

                if (token.Type == Language.TokenType.LineComment ||
                    token.Type == Language.TokenType.BlockCommentStart)
                {
                    commentStart = token.CharIndex;
                }
                else if (token.Type == Language.TokenType.String ||
                         token.Type == Language.TokenType.MultilineStringStart)
                {
                    stringStart = token.CharIndex;
                }
                else if (token.Type == Language.TokenType.FoldingStart)
                {
                    _foldingStacks[token.TokenIndex].Push(line.Index);
                }
                else if (token.Type == Language.TokenType.FoldingEnd)
                {
                    if (_foldingStacks[token.TokenIndex].TryPop(out int start))
                    {
                        int end = line.Index;
                        if (start < end)
                        {
                            _foldingList.Add(new Folding() { Start = start, End = end, Folded = false });
                        }
                    }
                }
            }

            // If there are tokens left until the end of the line, process them as comment or string until the end of the line
            if (commentStart >= 0)
            {
                line.CommentRanges.Add(new ValueTuple<int, int>(commentStart, line.String.Length));
            }
            if (stringStart >= 0)
            {
                line.StringRanges.Add(new ValueTuple<int, int>(stringStart, line.String.Length));
            }
            
            // Remove tokens that cannot cross lines
            while (_tokens.TryPeek(out Language.Token lineTop) &&
                   (lineTop.Type == Language.TokenType.LineComment || lineTop.Type == Language.TokenType.String))
            {
                _tokens.Pop();
            }
        }
            
        // Adding remaining foldings
        foreach (Stack<int> foldingStack in _foldingStacks.Values)
        {
            while (foldingStack.TryPop(out int start))
            {
                int end = Lines.Count - 1;
                if (start < end)
                {
                    _foldingList.Add(new Folding() { Start = start, End = end, Folded = false });
                }
            }
        }
        
        RestoreFolded_();
        
        _isLanguageDirty = false;
    }

    private void ShiftFoldingLine_(int lineIndex, EditDirection direction)
    {
        int moveCount = EditDirection.Forward == direction ? 1 : -1;
        foreach (Folding folding in _foldingList)
        {
            if (lineIndex <= folding.Start)
            {
                folding.Start += moveCount;
            }

            if (lineIndex <= folding.End)
            {
                folding.End += moveCount;
            }
            
            Logger.Info("ShiftFoldingLine: " + folding.Start + " " + folding.End + " " + moveCount);
        }
    }

    private void Unfold_(int lineIndex)
    {
        foreach (Folding folding in _foldingList)
        {
            if (folding.Start <= lineIndex && lineIndex <= folding.End)
            {
                folding.Folded = false;
            }
        }
    }

    private void SaveFolded_()
    {
        _foldedList.Clear();
        foreach (Folding folding in _foldingList)
        {
            if (folding.Folded)
            {
                _foldedList.Add(folding);
            }
        }
    }
    
    private void RestoreFolded_()
    {
        foreach (Folding folding in _foldingList)
        {
            foreach (Folding folded in _foldedList)
            {
                if (folding.Start == folded.Start && folding.End == folded.End)
                {
                    folding.Folded = true;
                }
            }
        }
    }
}