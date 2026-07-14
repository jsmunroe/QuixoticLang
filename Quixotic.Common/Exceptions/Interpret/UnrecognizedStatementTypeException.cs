using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnrecognizedStatementTypeException(Type statementType, Span span) : InterpreterException($"{statementType.Name} is not a recognized type of statement.", span);
}
