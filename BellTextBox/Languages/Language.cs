using Bell.Data;
using Bell.Themes;

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

    public ColorStyle CommentStyle = new();
    public ColorStyle TextStyle = new();

    public Dictionary<string, ColorStyle> PatternsStyle = new();
    public Dictionary<string, ColorStyle> KeywordsStyle = new();
    
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

    internal bool FindMatching(string source, int charIndex, out string matchedStr, out Token matchedToken)
    {
        matchedStr = string.Empty;
        matchedToken = new();

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
                    matchedStr = tokenString;

                    matchedToken.Type = tokenType;
                    matchedToken.TokenIndex = tokenIndex;
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
        internal int CharIndex;

        public bool Equals(Token other)
        {
            return Type == other.Type && TokenIndex == other.TokenIndex && CharIndex == other.CharIndex;
        }

        public override bool Equals(object? obj)
        {
            return obj is Token other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, TokenIndex, CharIndex);
        }
    }
}