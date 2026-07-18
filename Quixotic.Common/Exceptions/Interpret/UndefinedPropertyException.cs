using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedPropertyException(object targetType, string propertyName, Span span) : InterpreterException($"Type '{targetType}' does not have a '{propertyName}' property.", span);
}
