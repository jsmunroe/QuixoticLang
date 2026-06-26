using Quixotic.Common.Diagnostics;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class TokenException(string message, Token token, Diagnostic diagnostic) : ParserException(message, diagnostic)
    {
        public Token Token { get; } = token;

        public Span Span { get; set; } = token.Span;
    }
}
