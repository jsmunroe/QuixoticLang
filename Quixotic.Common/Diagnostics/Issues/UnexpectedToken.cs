using Quixotic.Common.Tokens;

namespace Quixotic.Common.Diagnostics.Issues
{
    public record UnexpectedToken(Token Encountered, TokenType? Expected = null) : Issue { }
}
