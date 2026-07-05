using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class ForLoopRangeTypeException(object type, string part, Span span) : SemanticException($"For loop range {part} value cannot be {type}", span, Severity.Error);
}
