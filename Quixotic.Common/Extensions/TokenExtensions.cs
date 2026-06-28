namespace Quixotic.Common.Tokens
{
    public static class TokenExtensions
    {
        public static Span GetTotalSpan(this IEnumerable<Token> tokens)
        {
            return tokens.Select(t => t.Span).GetTotalSpan();
        }

        public static Span GetTotalSpan(this IEnumerable<Span> spans)
        {
            Position min = Position.Empty;
            Position max = Position.Empty;

            foreach (var span in spans)
            {
                if (min.IsEmpty || min.Index > span.Start.Index)
                    min = span.Start;

                if (max.IsEmpty || max.Index < span.End.Index)
                    max = span.End;
            }

            if (min.IsEmpty || max.IsEmpty)
                return Span.Empty;

            return new Span
            {
                Start = min,
                End = max
            };
        }

        public static string GetValue(this IEnumerable<Token> tokens)
        {
            return string.Join(' ', tokens.Where(t => !t.IsTerminator).Select(t => t.Value));
        }

        public static IEnumerable<string> FindValues(this IEnumerable<Token> tokens, TokenType type)
        {
            foreach (var token in tokens)
            {
                if (token.Type == type)
                    yield return token.Value;
            }
        }
    }
}
