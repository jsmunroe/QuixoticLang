using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class UnexpectedTokenException(Token token) : TokenException($"Unexpected token '{token}'", token);
}
