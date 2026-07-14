using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class BaseCallOnTypeWithoutBaseTypeException(object type, Span span) : InterpreterException($"The type '{type}' does not have a base type. The base constructor cannot be invoked.", span);
}
