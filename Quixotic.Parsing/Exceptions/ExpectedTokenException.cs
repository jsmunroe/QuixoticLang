using Quixotic.Lexography.Tokens;

namespace Quixotic.Parsing.Exceptions
{
    public class ExpectedTokenException(TokenHead expected, Token token) : TokenException($"Expected token '{expected}' but received '{token}'", token);
}
