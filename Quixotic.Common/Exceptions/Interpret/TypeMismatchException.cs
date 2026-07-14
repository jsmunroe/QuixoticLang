using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class TypeMismatchException(object from, object to, Span span = new Span()) : InterpreterException($"Cannot convert from {from} to {to}.", span);
}
