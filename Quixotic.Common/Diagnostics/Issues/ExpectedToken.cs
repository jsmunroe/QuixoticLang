using Quixotic.Common.Tokens;

namespace Quixotic.Common.Diagnostics.Issues
{
    public record ExpectedToken(TokenType ExpectedTokenType) : Issue { }
}
