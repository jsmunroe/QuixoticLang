using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedMethodException(object targetType, string methodName, Span span) : InterpreterException($"Type '{targetType}' does not have a method named '{methodName}' that accepts the provided argument types.", span);
}
