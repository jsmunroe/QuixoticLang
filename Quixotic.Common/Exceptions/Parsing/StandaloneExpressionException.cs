using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class StandaloneExpressionException(Diagnostic diagnostic) : ParserException("The expression cannot stand alone as a statement", diagnostic);
}
