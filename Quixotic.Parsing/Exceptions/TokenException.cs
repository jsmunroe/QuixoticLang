using Quixotic.Lexography.Tokens;

namespace Quixotic.Parsing.Exceptions
{
    public class TokenException(string message, Token token) : ParserException(message)
    {
        public Token Token { get; } = token;

        public Position Position { get; set; } = token.Position;
    }
}
