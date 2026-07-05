using Quixotic.Analysis.Semantics;
using Quixotic.Common.Operations;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedOperatorException(Operator op, Span span) : SemanticException($"The operator '{op} is unrecognized and cannot be executed.", span, Severity.Error);
}
