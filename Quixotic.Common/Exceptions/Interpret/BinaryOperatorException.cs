using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class BinaryOperatorException(object left, string @operator, object right, Span span) : InterpreterException($"{@operator} is not supported between values of type {left} and {right}.", span);
}
