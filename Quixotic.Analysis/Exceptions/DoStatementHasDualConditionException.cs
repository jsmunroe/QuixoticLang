using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class DoStatementHasDualConditionException(Span span) : SemanticException("Do statement has both an entry and an exit condition.", span, Severity.Error);

}
