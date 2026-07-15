using Quixotic.Common.Expressions;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class ExpressionException(Exception inner, QxExpression expression) : InterpreterException(inner, expression.Span);
}
