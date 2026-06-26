using Quixotic.Common.Diagnostics;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class ExpectedTokenException(TokenHead expected, Token token, Diagnostic diagnostic) : TokenException($"Expected token '{expected}' but received '{token}'", token, diagnostic);
}
