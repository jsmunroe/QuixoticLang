using Quixotic.Common.Diagnostics;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class UnexpectedTokenException(Token token, Diagnostic diagnostic) : TokenException($"Unexpected token '{token}'", token, diagnostic);
}
