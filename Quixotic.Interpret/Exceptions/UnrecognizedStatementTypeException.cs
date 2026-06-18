namespace Quixotic.Interpret.Exceptions
{
    public class UnrecognizedStatementTypeException(Type statementType) : InterpreterException($"{statementType.Name} is not a recognized type of statement.");
}
