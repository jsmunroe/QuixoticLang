namespace Quixotic.Interpret.Exceptions
{
    public class BinaryOperatorException(Values.ValueType left, string @operator, Values.ValueType right) : InterpreterException($"{@operator} is not supported between values of type {left.Name} and {right.Name}.");
}
