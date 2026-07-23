using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class DoStatementMissingConditionException(Span span) : SemanticException("Do statement is missing a condition.", span, Severity.Error);

}
