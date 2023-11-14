using System.Text.RegularExpressions;
using Bell.Data;
using Bell.Inputs;

namespace Bell.Languages;

public partial class Language
{
    internal readonly Dictionary<TokenType, List<string>> Tokens = new()
    {
        { TokenType.LineComment, new() },
        { TokenType.BlockCommentStart, new() },
        { TokenType.BlockCommentEnd, new() },
        { TokenType.String, new() },
        { TokenType.MultilineStringStart, new() },
        { TokenType.MultilineStringEnd, new() },
        { TokenType.FoldingStart, new() },
        { TokenType.FoldingEnd, new() },
    };

    public ColorStyle DefaultStyle = new();
    public ColorStyle CommentStyle = new();
    public ColorStyle StringStyle = new();

    internal Dictionary<Regex, ColorStyle> PatternsStyle = new();
    
    public string AutoIndentPattern = "";

    internal enum TokenType
    {
        LineComment,
        BlockCommentStart,
        BlockCommentEnd,
        String,
        MultilineStringStart,
        MultilineStringEnd,
        FoldingStart,
        FoldingEnd
    }

    public void AddLineComment(string str)
    {
        Tokens[TokenType.LineComment].Add(str);
    }

    public void AddBlockComment(string startStr, string endStr)
    {
        Tokens[TokenType.BlockCommentStart].Add(startStr);
        Tokens[TokenType.BlockCommentEnd].Add(endStr);
    }
    
    public void AddString(string str)
    {
        Tokens[TokenType.String].Add(str);
    }
    
    public void AddMultilineString(string startStr, string endStr)
    {
        Tokens[TokenType.MultilineStringStart].Add(startStr);
        Tokens[TokenType.MultilineStringEnd].Add(endStr);
    }
    
    public void AddFolding(string startStr, string endStr)
    {
        Tokens[TokenType.FoldingStart].Add(startStr);
        Tokens[TokenType.FoldingEnd].Add(endStr);
    }
    
    public void AddPattern(string regex, ColorStyle colorStyle)
    {
        PatternsStyle.Add(new Regex(regex), colorStyle);
    }
    
    internal bool FindMatching(string source, int lineIndex, int charIndex, out Token matchedToken)
    {
        matchedToken = new Token();

        foreach (KeyValuePair<TokenType, List<string>> kv in Tokens)
        {
            TokenType tokenType = kv.Key;
            List<string> tokenStringList = kv.Value;

            for (int tokenIndex = 0; tokenIndex < tokenStringList.Count; tokenIndex++)
            {
                string tokenString = tokenStringList[tokenIndex];
                
                if (charIndex < 0 || charIndex + tokenString.Length > source.Length)
                    continue;

                bool isSame = true;
                for (int i = 0; i < tokenString.Length; i++)
                {
                    if (source[charIndex + i] != tokenString[i])
                    {
                        isSame = false;
                        break;
                    }
                }

                if (isSame)
                {
                    matchedToken.Type = tokenType;
                    matchedToken.TokenIndex = tokenIndex;
                    
                    matchedToken.TokenString = tokenString;
                    
                    matchedToken.LineIndex = lineIndex;
                    matchedToken.CharIndex = charIndex;
                    
                    return true;
                }
            }
        }
        return false;
    }
    
    internal struct Token : IEquatable<Token>
    {
        internal TokenType Type;
        internal int TokenIndex;

        internal string TokenString;
        
        internal int LineIndex;
        internal int CharIndex;

        public bool Equals(Token other)
        {
            return Type == other.Type && TokenIndex == other.TokenIndex && LineIndex == other.LineIndex && CharIndex == other.CharIndex;
        }

        public override bool Equals(object? obj)
        {
            return obj is Token other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, TokenIndex, LineIndex, CharIndex);
        }
    }
}