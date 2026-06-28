using Quixotic.Common.Diagnostics;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class UnexpectedTokenException : TokenException
    {
        public UnexpectedTokenException(Token encountered, Diagnostic diagnostic) : base($"Unexpected {encountered} encountered.", encountered, diagnostic)
        { }

        public UnexpectedTokenException(Token encountered, TokenType expected, Diagnostic diagnostic) : base($"Unexpected {encountered} encountered. '{expected}' was expected.", encountered, diagnostic)
        { }
    }
}
