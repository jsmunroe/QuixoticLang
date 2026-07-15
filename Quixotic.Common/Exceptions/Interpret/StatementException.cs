using Quixotic.Common.Statements;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class StatementException(Exception inner, QxStatement statement) : InterpreterException(inner, statement.Span);
}
