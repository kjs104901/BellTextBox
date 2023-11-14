using Bell.Actions;
using Bell.Languages;
using Bell.Themes;
using Bell.Utils;

namespace Bell.Data;

// Interface
internal partial class LineManager
{
    internal static List<Line> Lines => Singleton.TextBox.LineManager._lines;

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
}

// Implementation
internal partial class LineManager
{
    private readonly List<Line> _lines = new();

    private readonly List<Language.Token[]> _linesTokens = new();
    private readonly Stack<Language.Token> _tokens = new();

    private bool _isLanguageDirty = true;

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
    }

    private void UpdateLanguage_()
    {
        if (false == _isLanguageDirty)
            return;

        _tokens.Clear();
        foreach (Line line in _lines)
        {
            line.SyntaxList.Clear();

            foreach (Language.Token token in line.Tokens)
            {
                if (_tokens.TryPeek(out Language.Token top))
                {
                    // 이전 토큰이 끝나지 않은 경우 continue로 토큰 무시
                    if (top.Type == Language.TokenType.BlockCommentStart)
                    {
                        if (token.Type != Language.TokenType.BlockCommentEnd ||
                            token.TokenIndex != top.TokenIndex)
                            continue;

                        _tokens.Pop();
                        line.SyntaxList.Add(token);
                        continue;
                    }

                    if (top.Type == Language.TokenType.MultilineStringStart)
                    {
                        if (token.Type != Language.TokenType.MultilineStringEnd ||
                            token.TokenIndex != top.TokenIndex)
                            continue;

                        _tokens.Pop();
                        line.SyntaxList.Add(token);
                        continue;
                    }

                    if (top.Type == Language.TokenType.String &&
                        token.Type == Language.TokenType.String &&
                        token.TokenIndex == top.TokenIndex)
                    {
                        _tokens.Pop();
                        line.SyntaxList.Add(token);
                        continue;
                    }

                    if (top.Type == Language.TokenType.LineComment)
                    {
                        continue;
                    }
                }

                _tokens.Push(token);
                line.SyntaxList.Add(token);
            }

            // 라인을 넘을 수 없는 토큰은 제거
            while (_tokens.TryPeek(out Language.Token lineTop) &&
                   (lineTop.Type == Language.TokenType.LineComment || lineTop.Type == Language.TokenType.String))
            {
                _tokens.Pop();
            }
        }
    }
}