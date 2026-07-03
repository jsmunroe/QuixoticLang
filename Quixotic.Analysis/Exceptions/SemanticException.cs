using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class SemanticException(string message, Span span) : Exception(message)
    {
        public Span Span { get; } = span;
    }
}
