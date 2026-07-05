using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class SemanticException(string message, Span span, Severity severity) : Exception(message)
    {
        public Span Span { get; } = span;
        public Severity Severity { get; } = severity;
    }
}
