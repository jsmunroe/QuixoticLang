using Quixotic.Parsing.Expressions;

namespace Quixotic.Interpret.Exceptions
{
    public class BinaryOperatorException : InterpreterException
    {
        public BinaryOperatorException(ExpressionKind left, string @operator, ExpressionKind right)
            : base($"{@operator} is not supported between values of type {left} and {right}.")
        { }

        public BinaryOperatorException(Symbols.QxType left, string @operator, Symbols.QxType right)
            : base($"{@operator} is not supported between values of type {left.Name} and {right.Name}.")
        { }
    }
}
