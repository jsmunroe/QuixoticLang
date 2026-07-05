using Quixotic.Common.Tokens;

namespace Quixotic.Common.Syntax
{
    public record Keyword(TokenType TokenType, string Value)
    {
        private static List<Keyword> Keywords { get; } =
        [
            new (TokenType.Print , "print"),
            new (TokenType.Let , "let"),
            new (TokenType.If , "if"),
            new (TokenType.Then , "then"),
            new (TokenType.Else , "else"),
            new (TokenType.For , "for"),
            new (TokenType.To , "to"),
            new (TokenType.Step , "step"),
            new (TokenType.Next , "next"),
            new (TokenType.In , "in"),
            new (TokenType.Do , "do"),
            new (TokenType.While , "while"),
            new (TokenType.Until , "until"),
            new (TokenType.Loop , "loop"),
            new (TokenType.Continue , "continue"),
            new (TokenType.Break , "break"),
            new (TokenType.End , "end"),
            new (TokenType.True , "true"),
            new (TokenType.False , "false"),
            new (TokenType.And , "and"),
            new (TokenType.Or , "or"),
            new (TokenType.Not , "not"),
            new (TokenType.Function , "function"),
            new (TokenType.Return , "return"),
        ];

        private static Dictionary<string, Keyword> _keywordsByValue = Keywords.ToDictionary(k => k.Value, k => k);

        private static Dictionary<TokenType, Keyword> _keywordsByTokenType = Keywords.ToDictionary(k => k.TokenType, k => k);

        public static bool Contains(string value)
        {
            return _keywordsByValue.ContainsKey(value);
        }

        public static bool Contains(TokenType tokenType)
        {
            return _keywordsByTokenType.ContainsKey(tokenType);
        }

        public static Keyword? FromValue(string value)
        {
            return _keywordsByValue.TryGetValue(value, out var keyword) ? keyword : null;
        }

        public static Keyword? FromTokenType(TokenType tokenType)
        {
            return _keywordsByTokenType.TryGetValue(tokenType, out var keyword) ? keyword : null;
        }
    }
}
