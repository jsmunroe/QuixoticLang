using Quixotic.Common.Tokens;

namespace Quixotic.Common.Diagnostics.Issues
{
    public record UnexpectedToken(Token Token) : Issue { }
}
