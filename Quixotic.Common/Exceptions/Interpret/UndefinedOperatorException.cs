using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedOperatorException(object targetType, string operatorName, Span span) : InterpreterException($"Operator '{operatorName}' is not defined for values of type '{targetType}'.", span);
}
