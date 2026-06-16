using Quixotic.Lexography.Tokens;

namespace Quixotic.Parsing.Exceptions
{
    public class UnexpectedTokenException(Token token) : TokenException($"Unexpected token '{token}'", token);
}
