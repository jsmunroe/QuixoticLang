namespace Quixotic.Interpret.Exceptions
{
    public class ExpectedReturnValueException(string functionName) : InterpreterException($"The function '{functionName}' was expected to return a value but did not return one.");
}
