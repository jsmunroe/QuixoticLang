using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedFunctionException(string name, Span span) : InterpreterException($"No function named '{name}' has been defined.", span);
}
