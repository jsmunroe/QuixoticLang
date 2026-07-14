using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class OutOfRangeException(string name, Span span) : InterpreterException($"Value {name} is out of range.", span);
}
