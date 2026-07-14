using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class TypeHasNoBaseTypeException(object type, Span span) : InterpreterException($"Type '{type}' has no base type.", span);
}
