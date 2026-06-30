using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class StandaloneExpressionException(Diagnostic diagnostic) : ParserException("Standalone expressions are not allowed.", diagnostic);
}
