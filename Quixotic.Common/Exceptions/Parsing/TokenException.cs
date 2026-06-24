using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class TokenException(string message, Token token) : ParserException(message)
    {
        public Token Token { get; } = token;

        public Span Span { get; set; } = token.Span;
    }
}
