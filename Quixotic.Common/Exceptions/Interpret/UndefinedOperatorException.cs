namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedOperatorException(object targetType, string operatorName) : RuntimeException($"Operator '{operatorName}' is not defined for values of type '{targetType}'.");
}
