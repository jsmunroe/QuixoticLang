namespace Quixotic.Interpret.Exceptions
{
    public class BinaryOperatorException(Symbols.ValueType left, string @operator, Symbols.ValueType right) : InterpreterException($"{@operator} is not supported between values of type {left.Name} and {right.Name}.");
}
