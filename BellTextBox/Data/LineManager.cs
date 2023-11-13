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
}

// Implementation
internal partial class LineManager
{
    private readonly List<Line> _lines = new();
    
    private readonly List<Language.Token[]> _linesTokens = new();
    private readonly Stack<Language.Token> _tokens = new();

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
    
    private void UpdateLanguage_()
    {
        bool needUpdate = _lines.Count != _linesTokens.Count;
        if (false == needUpdate)
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                Line line = _lines[i];
                Language.Token[] tokens = _linesTokens[i];
                
                if (false == tokens.SequenceEqual(line.LanguageTokens))
                {
                    needUpdate = true;
                    break;
                }
            }
        }

        if (needUpdate)
        {
            _tokens.Clear();
            foreach (Line line in _lines)
            {
                line.ClearSyntax();
                
                // 최상위 스택이 있다면 추가
                line.AddSyntax(token);
                
                foreach (Language.Token token in line.LanguageTokens)
                {
                    _tokens.Push(token); // 스택으로 처리

                    // 무시당하지 않은 스택 추가
                    line.AddSyntax(token);
                }
            }
        }
    }
}