namespace Quixotic.Common.Exceptions.Interpret
{
    public class ExecutionException(string message, Exception innerException) : InterpreterException(message, innerException)
    { }
}
