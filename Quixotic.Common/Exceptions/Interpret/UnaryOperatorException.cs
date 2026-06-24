namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnaryOperatorException(string @operator) : InterpreterException($"{@operator} is not supported as a unary operator.");

    public class ExpressionTypeException(ValueType expected) : InterpreterException($"An {expected} expression was expected.");
}
