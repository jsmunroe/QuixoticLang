
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnexpectedTypeException(object expected, object actual, Span span) : InterpreterException($"{expected} type was expected, but {actual} type was received.", span);
}
