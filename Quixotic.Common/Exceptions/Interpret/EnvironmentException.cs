using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class EnvironmentException(string message, Span span) : Exception(message)
    {
        public Span Span { get; } = span;
    }
}
