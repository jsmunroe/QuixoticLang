using Quixotic.Common.Contracts;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class InterpreterException : Exception, IHasSpan
    {

        public InterpreterException(string message, Exception inner, Span span) : base(message, inner)
        {
            Span = span;
        }

        public InterpreterException(string message, Span span) : base(message)
        {
            Span = span;
        }

        public InterpreterException(Exception inner, Span span) : base(inner.Message)
        {
            Span = span;
        }

        public Span Span { get; }
    }
}
