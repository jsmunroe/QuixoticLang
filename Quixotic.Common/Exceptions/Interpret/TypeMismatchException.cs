using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class TypeMismatchException(object from, object to, Span span) : EnvironmentException($"Cannot convert from {from} to {to}.", span);
}
