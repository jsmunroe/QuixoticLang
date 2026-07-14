using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class ExpectedReturnValueException(string functionName, Span span) : InterpreterException($"The function '{functionName}' was expected to return a value but did not return one.", span);
}
